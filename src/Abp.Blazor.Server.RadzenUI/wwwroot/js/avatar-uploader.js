(function () {
    const editors = new Map();

    function getEditor(containerId) {
        const editor = editors.get(containerId);
        if (!editor) {
            throw new Error(`Avatar editor '${containerId}' is not initialized.`);
        }

        return editor;
    }

    function clamp(value, min, max) {
        return Math.min(Math.max(value, min), max);
    }

    function setStatus(editor, message, isError) {
        if (!editor.status) {
            return;
        }

        editor.status.textContent = message || "";
        editor.status.dataset.state = isError ? "error" : "info";
    }

    function getErrorMessage(payload, fallbackMessage) {
        if (!payload) {
            return fallbackMessage;
        }

        if (typeof payload.error === "string" && payload.error) {
            return payload.error;
        }

        if (payload.error && typeof payload.error.message === "string" && payload.error.message) {
            return payload.error.message;
        }

        if (typeof payload.message === "string" && payload.message) {
            return payload.message;
        }

        return fallbackMessage;
    }

    function setViewportDragging(editor, isDragging) {
        if (!editor.viewport) {
            return;
        }

        editor.viewport.classList.toggle("is-dragging", isDragging);
    }

    function applyTransform(editor) {
        if (!editor.image) {
            return;
        }

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
        const width = editor.state.imageWidth * editor.state.zoom;
        const height = editor.state.imageHeight * editor.state.zoom;
        const minX = Math.min(0, editor.options.viewportSize - width);
        const minY = Math.min(0, editor.options.viewportSize - height);

        editor.state.x = clamp(editor.state.x, minX, 0);
        editor.state.y = clamp(editor.state.y, minY, 0);
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
        if (!editor.state.fileLoaded) {
            return;
        }

        const safeOriginX = originX ?? editor.options.viewportSize / 2;
        const safeOriginY = originY ?? editor.options.viewportSize / 2;
        const imagePointX = (safeOriginX - editor.state.x) / editor.state.zoom;
        const imagePointY = (safeOriginY - editor.state.y) / editor.state.zoom;

        editor.state.zoom = clamp(nextZoom, editor.state.minZoom, editor.state.maxZoom);
        editor.state.x = safeOriginX - imagePointX * editor.state.zoom;
        editor.state.y = safeOriginY - imagePointY * editor.state.zoom;

        updatePreview(editor);
    }

    function startDragging(editor, event) {
        if (!editor.state.fileLoaded) {
            return;
        }

        editor.state.dragging = true;
        editor.state.pointerId = event.pointerId;
        editor.state.dragOriginX = event.clientX;
        editor.state.dragOriginY = event.clientY;
        editor.state.startX = editor.state.x;
        editor.state.startY = editor.state.y;

        if (editor.viewport.setPointerCapture) {
            editor.viewport.setPointerCapture(event.pointerId);
        }

        setViewportDragging(editor, true);
    }

    function moveDragging(editor, event) {
        if (!editor.state.dragging || editor.state.pointerId !== event.pointerId) {
            return;
        }

        editor.state.x = editor.state.startX + (event.clientX - editor.state.dragOriginX);
        editor.state.y = editor.state.startY + (event.clientY - editor.state.dragOriginY);
        updatePreview(editor);
    }

    function stopDragging(editor, event) {
        if (!editor.state.dragging || (event && editor.state.pointerId !== event.pointerId)) {
            return;
        }

        if (event && editor.viewport.releasePointerCapture) {
            editor.viewport.releasePointerCapture(event.pointerId);
        }

        editor.state.dragging = false;
        editor.state.pointerId = null;
        setViewportDragging(editor, false);
    }

    async function loadSelectedFile(editor) {
        const file = editor.input.files && editor.input.files[0];
        if (!file) {
            return { success: false, error: editor.options.messages.noFileSelected };
        }

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

        if (editor.state.objectUrl) {
            URL.revokeObjectURL(editor.state.objectUrl);
        }

        const objectUrl = URL.createObjectURL(file);
        editor.state.objectUrl = objectUrl;

        await new Promise((resolve, reject) => {
            editor.image.onload = () => resolve();
            editor.image.onerror = () => reject(new Error(editor.options.messages.previewFailed));
            editor.image.src = objectUrl;
        });

        editor.state.file = file;
        editor.state.fileLoaded = true;
        editor.state.imageWidth = editor.image.naturalWidth;
        editor.state.imageHeight = editor.image.naturalHeight;

        const minZoom = Math.max(
            editor.options.viewportSize / editor.state.imageWidth,
            editor.options.viewportSize / editor.state.imageHeight
        );
        const maxZoom = Math.max(minZoom * 3, minZoom + 2);

        editor.state.minZoom = minZoom;
        editor.state.maxZoom = maxZoom;
        editor.state.zoom = minZoom;

        const scaledWidth = editor.state.imageWidth * minZoom;
        const scaledHeight = editor.state.imageHeight * minZoom;
        editor.state.x = (editor.options.viewportSize - scaledWidth) / 2;
        editor.state.y = (editor.options.viewportSize - scaledHeight) / 2;

        updatePreview(editor);
        setStatus(editor, file.name, false);

        const placeholder = editor.viewport.querySelector('.avatar-editor__placeholder');
        if (placeholder) placeholder.style.display = 'none';

        return { success: true };
    }

    async function buildCanvasBlob(editor) {
        const canvas = document.createElement("canvas");
        canvas.width = editor.options.outputSize;
        canvas.height = editor.options.outputSize;

        const context = canvas.getContext("2d");
        context.clearRect(0, 0, canvas.width, canvas.height);

        const scale = editor.options.outputSize / editor.options.viewportSize;
        context.drawImage(
            editor.image,
            editor.state.x * scale,
            editor.state.y * scale,
            editor.state.imageWidth * editor.state.zoom * scale,
            editor.state.imageHeight * editor.state.zoom * scale
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
            if (!container) {
                throw new Error(`Avatar editor container '${containerId}' was not found.`);
            }

            const editor = {
                container,
                viewport: container.querySelector(".avatar-editor__viewport"),
                input: document.getElementById(options.inputId),
                image: document.getElementById(options.imageId),
                status: document.getElementById(options.statusId),
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
                    dragging: false,
                    pointerId: null,
                    dragOriginX: 0,
                    dragOriginY: 0,
                    startX: 0,
                    startY: 0,
                    x: 0,
                    y: 0
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
                if (!editor.state.fileLoaded) {
                    return;
                }

                event.preventDefault();
                startDragging(editor, event);
            });

            editor.viewport.addEventListener("pointermove", event => {
                moveDragging(editor, event);
            });

            editor.viewport.addEventListener("pointerup", event => {
                stopDragging(editor, event);
            });

            editor.viewport.addEventListener("pointercancel", event => {
                stopDragging(editor, event);
            });

            editor.viewport.addEventListener("wheel", event => {
                if (!editor.state.fileLoaded) {
                    return;
                }

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

            // Read actual rendered viewport size for zoom / drag maths
            // Use requestAnimationFrame so the Radzen dialog has time to layout
            requestAnimationFrame(function () {
                const actualSize = editor.viewport.clientWidth;
                if (actualSize > 0) {
                    editor.options.viewportSize = actualSize;
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
            editor.state.file = null;
            editor.state.fileLoaded = false;
            editor.state.dragging = false;
            editor.state.pointerId = null;
            editor.state.zoom = 1;
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

            setViewportDragging(editor, false);
            applyTransform(editor);

            const placeholder = editor.viewport.querySelector('.avatar-editor__placeholder');
            if (placeholder) placeholder.style.display = '';

            setStatus(
                editor,
                "",
                false
            );
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