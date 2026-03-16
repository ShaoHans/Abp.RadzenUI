(function () {
    const editors = new Map();

    function getEditor(containerId) {
        const editor = editors.get(containerId);
        if (!editor) throw new Error(`Avatar editor '${containerId}' is not initialized.`);
        return editor;
    }

    function clamp(value, min, max) {
        return Math.min(Math.max(value, min), max);
    }

    function setStatus(editor, message, isError) {
        if (!editor.status) return;
        editor.status.textContent = message || "";
        editor.status.dataset.state = isError ? "error" : "info";
    }

    function getErrorMessage(payload, fallbackMessage) {
        if (!payload) return fallbackMessage;
        if (typeof payload.error === "string" && payload.error) return payload.error;
        if (payload.error && typeof payload.error.message === "string" && payload.error.message) return payload.error.message;
        if (typeof payload.message === "string" && payload.message) return payload.message;
        return fallbackMessage;
    }

    // Convert pixel coordinate to SVG 0-100 coordinate space
    function pxToSvg(px, viewportSize) {
        return (px / viewportSize) * 100;
    }

    function updateCropCircleSvg(editor) {
        const vs = editor.options.viewportSize;
        const { cropX, cropY, cropR } = editor.state;
        if (editor.maskCircle) {
            editor.maskCircle.setAttribute('cx', pxToSvg(cropX, vs));
            editor.maskCircle.setAttribute('cy', pxToSvg(cropY, vs));
            editor.maskCircle.setAttribute('r',  pxToSvg(cropR, vs));
        }
        if (editor.strokeCircle) {
            editor.strokeCircle.setAttribute('cx', pxToSvg(cropX, vs));
            editor.strokeCircle.setAttribute('cy', pxToSvg(cropY, vs));
            editor.strokeCircle.setAttribute('r',  pxToSvg(cropR, vs));
        }
        if (editor.resizeHandle) {
            editor.resizeHandle.setAttribute('cx', pxToSvg(cropX, vs));
            editor.resizeHandle.setAttribute('cy', pxToSvg(cropY + cropR, vs));
        }
    }

    function constrainCropCircle(editor) {
        const vs   = editor.options.viewportSize;
        const minR = editor.state.minCropR;
        const maxR = vs * 0.48;
        editor.state.cropR = clamp(editor.state.cropR, minR, maxR);
        const r = editor.state.cropR;
        editor.state.cropX = clamp(editor.state.cropX, r, vs - r);
        editor.state.cropY = clamp(editor.state.cropY, r, vs - r);
    }

    function initCropCircle(editor) {
        const vs = editor.options.viewportSize;
        editor.state.cropX    = vs / 2;
        editor.state.cropY    = vs / 2;
        editor.state.cropR    = vs * 0.4;
        editor.state.minCropR = vs * 0.15;
        updateCropCircleSvg(editor);
    }

    function applyTransform(editor) {
        if (!editor.image) return;
        if (!editor.state.fileLoaded) {
            editor.image.style.transform = "";
            editor.image.style.width = "100%";
            editor.image.style.height = "100%";
            editor.image.style.objectFit = editor.image.getAttribute("src") ? "cover" : "contain";
            editor.image.style.transformOrigin = "center center";
            editor.viewport.classList.remove("is-editable");
            return;
        }
        const { x, y, zoom } = editor.state;
        editor.image.style.width = `${editor.state.imageWidth}px`;
        editor.image.style.height = `${editor.state.imageHeight}px`;
        editor.image.style.objectFit = "unset";
        editor.image.style.transformOrigin = "top left";
        editor.image.style.transform = `translate(${x}px, ${y}px) scale(${zoom})`;
        editor.viewport.classList.add("is-editable");
    }

    function constrainPosition(editor) {
        const { cropX, cropY, cropR, zoom } = editor.state;
        const imgW = editor.state.imageWidth  * zoom;
        const imgH = editor.state.imageHeight * zoom;
        // Image must fully cover the crop circle area
        editor.state.x = clamp(editor.state.x, (cropX + cropR) - imgW, cropX - cropR);
        editor.state.y = clamp(editor.state.y, (cropY + cropR) - imgH, cropY - cropR);
    }

    function updatePreview(editor) {
        if (!editor.state.fileLoaded) {
            applyTransform(editor);
            return;
        }
        constrainPosition(editor);
        applyTransform(editor);
    }

    function setZoom(editor, nextZoom, originX, originY) {
        if (!editor.state.fileLoaded) return;
        const safeOriginX = originX ?? editor.options.viewportSize / 2;
        const safeOriginY = originY ?? editor.options.viewportSize / 2;
        const imagePointX = (safeOriginX - editor.state.x) / editor.state.zoom;
        const imagePointY = (safeOriginY - editor.state.y) / editor.state.zoom;
        editor.state.zoom = clamp(nextZoom, editor.state.minZoom, editor.state.maxZoom);
        editor.state.x = safeOriginX - imagePointX * editor.state.zoom;
        editor.state.y = safeOriginY - imagePointY * editor.state.zoom;
        updatePreview(editor);
    }

    // Returns: 'circleResize' | 'circleDrag' | 'imageDrag' | null
    function getInteractionMode(editor, rect, clientX, clientY) {
        const x  = clientX - rect.left;
        const y  = clientY - rect.top;
        const dx = x - editor.state.cropX;
        const dy = y - editor.state.cropY;
        const dist = Math.sqrt(dx * dx + dy * dy);
        const RESIZE_ZONE = 20; // px
        if (dist >= editor.state.cropR - RESIZE_ZONE && dist <= editor.state.cropR + RESIZE_ZONE) {
            return 'circleResize';
        }
        if (dist < editor.state.cropR - RESIZE_ZONE) {
            return 'circleDrag';
        }
        return editor.state.fileLoaded ? 'imageDrag' : null;
    }

    function startInteraction(editor, event) {
        const rect = editor.viewport.getBoundingClientRect();
        const mode = getInteractionMode(editor, rect, event.clientX, event.clientY);
        if (!mode) return;
        event.preventDefault();
        editor.state.interactionMode = mode;
        editor.state.pointerId   = event.pointerId;
        editor.state.dragOriginX = event.clientX;
        editor.state.dragOriginY = event.clientY;
        editor.state.startX      = editor.state.x;
        editor.state.startY      = editor.state.y;
        editor.state.startCropX  = editor.state.cropX;
        editor.state.startCropY  = editor.state.cropY;
        editor.state.startCropR  = editor.state.cropR;
        if (editor.viewport.setPointerCapture) {
            editor.viewport.setPointerCapture(event.pointerId);
        }
        if (mode === 'imageDrag') {
            editor.viewport.classList.add("is-dragging");
        }
    }

    function moveInteraction(editor, event) {
        if (!editor.state.interactionMode || editor.state.pointerId !== event.pointerId) return;
        const dx = event.clientX - editor.state.dragOriginX;
        const dy = event.clientY - editor.state.dragOriginY;

        if (editor.state.interactionMode === 'imageDrag') {
            editor.state.x = editor.state.startX + dx;
            editor.state.y = editor.state.startY + dy;
            updatePreview(editor);
        } else if (editor.state.interactionMode === 'circleDrag') {
            editor.state.cropX = editor.state.startCropX + dx;
            editor.state.cropY = editor.state.startCropY + dy;
            constrainCropCircle(editor);
            if (editor.state.fileLoaded) {
                constrainPosition(editor);
                applyTransform(editor);
            }
            updateCropCircleSvg(editor);
        } else if (editor.state.interactionMode === 'circleResize') {
            const r2  = editor.viewport.getBoundingClientRect();
            const px  = event.clientX - r2.left;
            const py  = event.clientY - r2.top;
            const ddx = px - editor.state.cropX;
            const ddy = py - editor.state.cropY;
            editor.state.cropR = Math.sqrt(ddx * ddx + ddy * ddy);
            constrainCropCircle(editor);
            if (editor.state.fileLoaded) {
                constrainPosition(editor);
                applyTransform(editor);
            }
            updateCropCircleSvg(editor);
        }
    }

    function stopInteraction(editor, event) {
        if (!editor.state.interactionMode || (event && editor.state.pointerId !== event.pointerId)) return;
        if (event && editor.viewport.releasePointerCapture) {
            editor.viewport.releasePointerCapture(event.pointerId);
        }
        editor.viewport.classList.remove("is-dragging");
        editor.state.interactionMode = null;
        editor.state.pointerId = null;
        editor.viewport.style.cursor = '';
    }

    async function loadSelectedFile(editor) {
        const file = editor.input.files && editor.input.files[0];
        if (!file) return { success: false, error: editor.options.messages.noFileSelected };

        if (!editor.options.allowedContentTypes.includes(file.type)) {
            setStatus(editor, editor.options.messages.invalidFileType, true);
            editor.input.value = "";
            return { success: false, error: editor.options.messages.invalidFileType };
        }

        if (file.size > editor.options.maxFileSize) {
            setStatus(editor, editor.options.messages.fileTooLarge, true);
            editor.input.value = "";
            return { success: false, error: editor.options.messages.fileTooLarge };
        }

        if (editor.state.objectUrl) URL.revokeObjectURL(editor.state.objectUrl);

        const objectUrl = URL.createObjectURL(file);
        editor.state.objectUrl = objectUrl;

        await new Promise((resolve, reject) => {
            editor.image.onload = () => resolve();
            editor.image.onerror = () => reject(new Error(editor.options.messages.previewFailed));
            editor.image.src = objectUrl;
        });

        editor.state.file = file;
        editor.state.fileLoaded = true;
        editor.state.imageWidth  = editor.image.naturalWidth;
        editor.state.imageHeight = editor.image.naturalHeight;

        const vs = editor.options.viewportSize;
        const minZoom = Math.max(vs / editor.state.imageWidth, vs / editor.state.imageHeight);
        const maxZoom = Math.max(minZoom * 3, minZoom + 2);

        editor.state.minZoom = minZoom;
        editor.state.maxZoom = maxZoom;
        editor.state.zoom    = minZoom;

        const scaledWidth  = editor.state.imageWidth  * minZoom;
        const scaledHeight = editor.state.imageHeight * minZoom;
        editor.state.x = (vs - scaledWidth)  / 2;
        editor.state.y = (vs - scaledHeight) / 2;

        updatePreview(editor);
        setStatus(editor, file.name, false);

        const placeholder = editor.viewport.querySelector('.avatar-editor__placeholder');
        if (placeholder) placeholder.style.display = 'none';

        return { success: true };
    }

    async function buildCanvasBlob(editor) {
        const canvas     = document.createElement("canvas");
        const outputSize = editor.options.outputSize;
        canvas.width  = outputSize;
        canvas.height = outputSize;
        const context = canvas.getContext("2d");
        context.clearRect(0, 0, outputSize, outputSize);

        // Crop only the circular area; scale it to fill the output canvas
        const { cropX, cropY, cropR, x, y, zoom, imageWidth, imageHeight } = editor.state;
        const cropLeft    = cropX - cropR;
        const cropTop     = cropY - cropR;
        const circleScale = outputSize / (cropR * 2);
        context.drawImage(
            editor.image,
            (x - cropLeft) * circleScale,
            (y - cropTop)  * circleScale,
            imageWidth  * zoom * circleScale,
            imageHeight * zoom * circleScale
        );

        const targetMimeType = editor.state.file.type === "image/png" ? "image/png" : "image/jpeg";
        const quality = targetMimeType === "image/png" ? undefined : 0.92;

        return await new Promise((resolve, reject) => {
            canvas.toBlob(blob => {
                if (!blob) {
                    reject(new Error(editor.options.messages.previewFailed));
                    return;
                }
                resolve({ blob, targetMimeType });
            }, targetMimeType, quality);
        });
    }

    window.abpRadzenAvatar = {
        initialize: function (containerId, options) {
            const container = document.getElementById(containerId);
            if (!container) throw new Error(`Avatar editor container '${containerId}' was not found.`);

            const editor = {
                container,
                viewport:     container.querySelector(".avatar-editor__viewport"),
                input:        document.getElementById(options.inputId),
                image:        document.getElementById(options.imageId),
                status:       document.getElementById(options.statusId),
                maskCircle:   container.querySelector(".avatar-editor__mask-circle"),
                strokeCircle: container.querySelector(".avatar-editor__stroke-circle"),
                resizeHandle: container.querySelector(".avatar-editor__resize-handle"),
                options,
                state: {
                    file: null,
                    fileLoaded: false,
                    objectUrl: null,
                    imageWidth: 0,
                    imageHeight: 0,
                    zoom: 1,
                    minZoom: 1,
                    maxZoom: 1,
                    interactionMode: null, // 'imageDrag' | 'circleDrag' | 'circleResize'
                    pointerId: null,
                    dragOriginX: 0,
                    dragOriginY: 0,
                    startX: 0,
                    startY: 0,
                    x: 0,
                    y: 0,
                    cropX: 0,
                    cropY: 0,
                    cropR: 0,
                    minCropR: 0,
                    startCropX: 0,
                    startCropY: 0,
                    startCropR: 0
                }
            };

            if (!editor.viewport || !editor.input || !editor.image) {
                throw new Error(`Avatar editor '${containerId}' is missing required elements.`);
            }

            editor.input.addEventListener("change", async () => {
                try {
                    await loadSelectedFile(editor);
                } catch (error) {
                    setStatus(editor, error.message || editor.options.messages.previewFailed, true);
                }
            });

            editor.viewport.addEventListener("pointerdown", event => {
                startInteraction(editor, event);
            });

            editor.viewport.addEventListener("pointermove", event => {
                if (editor.state.interactionMode) {
                    moveInteraction(editor, event);
                    // Maintain appropriate cursor during drag
                    editor.viewport.style.cursor =
                        editor.state.interactionMode === 'circleResize' ? 'nwse-resize' : 'grabbing';
                } else {
                    // Hover: show cursor hint based on pointer position
                    const rect = editor.viewport.getBoundingClientRect();
                    const mode = getInteractionMode(editor, rect, event.clientX, event.clientY);
                    if      (mode === 'circleDrag')    editor.viewport.style.cursor = 'move';
                    else if (mode === 'circleResize')  editor.viewport.style.cursor = 'nwse-resize';
                    else if (mode === 'imageDrag')     editor.viewport.style.cursor = 'grab';
                    else                               editor.viewport.style.cursor = '';
                }
            });

            editor.viewport.addEventListener("pointerup", event => {
                stopInteraction(editor, event);
            });

            editor.viewport.addEventListener("pointercancel", event => {
                stopInteraction(editor, event);
            });

            editor.viewport.addEventListener("mouseleave", () => {
                if (!editor.state.interactionMode) editor.viewport.style.cursor = '';
            });

            editor.viewport.addEventListener("wheel", event => {
                if (!editor.state.fileLoaded) return;
                event.preventDefault();
                const rect = editor.viewport.getBoundingClientRect();
                const zoomFactor = event.deltaY < 0 ? 1.08 : 0.92;
                setZoom(
                    editor,
                    editor.state.zoom * zoomFactor,
                    event.clientX - rect.left,
                    event.clientY - rect.top
                );
            }, { passive: false });

            // Initialise crop circle; correct with actual rendered size after layout
            initCropCircle(editor);
            requestAnimationFrame(function () {
                const actualSize = editor.viewport.clientWidth;
                if (actualSize > 0 && actualSize !== editor.options.viewportSize) {
                    editor.options.viewportSize = actualSize;
                    initCropCircle(editor);
                }
            });

            applyTransform(editor);
            editors.set(containerId, editor);
        },

        pickFile: function (containerId) {
            getEditor(containerId).input.click();
        },

        resetSelection: function (containerId) {
            const editor = getEditor(containerId);
            editor.input.value = "";
            editor.state.file            = null;
            editor.state.fileLoaded      = false;
            editor.state.interactionMode = null;
            editor.state.pointerId       = null;
            editor.state.zoom    = 1;
            editor.state.minZoom = 1;
            editor.state.maxZoom = 1;
            editor.state.x = 0;
            editor.state.y = 0;

            if (editor.state.objectUrl) {
                URL.revokeObjectURL(editor.state.objectUrl);
                editor.state.objectUrl = null;
            }

            const initialSrc = editor.image.dataset.initialSrc || "";
            if (initialSrc) {
                editor.image.src = initialSrc;
            } else {
                editor.image.removeAttribute("src");
            }

            editor.viewport.classList.remove("is-dragging");
            editor.viewport.style.cursor = '';
            applyTransform(editor);
            initCropCircle(editor);

            const placeholder = editor.viewport.querySelector('.avatar-editor__placeholder');
            if (placeholder) placeholder.style.display = '';

            setStatus(editor, "", false);
        },

        zoomIn: function (containerId) {
            const editor = getEditor(containerId);
            setZoom(editor, editor.state.zoom * 1.12);
        },

        zoomOut: function (containerId) {
            const editor = getEditor(containerId);
            setZoom(editor, editor.state.zoom * 0.88);
        },

        upload: async function (containerId, endpoint) {
            const editor = getEditor(containerId);
            if (!editor.state.fileLoaded || !editor.state.file) {
                return { success: false, error: editor.options.messages.noFileSelected };
            }

            try {
                const { blob, targetMimeType } = await buildCanvasBlob(editor);
                const extension = targetMimeType === "image/png" ? "png" : "jpg";
                const formData = new FormData();
                formData.append("file", blob, `avatar.${extension}`);

                const response = await fetch(endpoint, {
                    method: "POST",
                    body: formData,
                    credentials: "same-origin"
                });

                if (!response.ok) {
                    const payload = await response.json().catch(() => null);
                    return {
                        success: false,
                        error: getErrorMessage(payload, editor.options.messages.uploadFailed)
                    };
                }

                const payload = await response.json();
                editor.image.dataset.initialSrc = payload.avatarUrl || "";
                this.resetSelection(containerId);
                return { success: true, avatarUrl: payload.avatarUrl };
            } catch (error) {
                return { success: false, error: error.message || editor.options.messages.uploadFailed };
            }
        },

        deleteAvatar: async function (containerId, endpoint) {
            const editor = getEditor(containerId);

            try {
                const response = await fetch(endpoint, {
                    method: "DELETE",
                    credentials: "same-origin"
                });

                if (!response.ok) {
                    const payload = await response.json().catch(() => null);
                    return {
                        success: false,
                        error: getErrorMessage(payload, editor.options.messages.deleteFailed)
                    };
                }

                editor.image.dataset.initialSrc = "";
                editor.image.removeAttribute("src");
                this.resetSelection(containerId);
                return { success: true };
            } catch (error) {
                return { success: false, error: error.message || editor.options.messages.deleteFailed };
            }
        },

        deleteAvatarApi: async function (endpoint, errorMessage) {
            try {
                const response = await fetch(endpoint, {
                    method: "DELETE",
                    credentials: "same-origin"
                });

                if (!response.ok) {
                    const payload = await response.json().catch(() => null);
                    return {
                        success: false,
                        error: getErrorMessage(payload, errorMessage || "Delete failed")
                    };
                }

                return { success: true };
            } catch (error) {
                return { success: false, error: error.message || errorMessage || "Delete failed" };
            }
        }
    };
})();
