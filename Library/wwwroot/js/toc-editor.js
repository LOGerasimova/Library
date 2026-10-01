(() => {
    "use strict";

    const listTags = new Set(["OL", "UL"]);

    function directText(listItem) {
        return Array.from(listItem.childNodes)
            .filter(node => !(node.nodeType === Node.ELEMENT_NODE && listTags.has(node.tagName)))
            .map(node => node.textContent)
            .join(" ")
            .replace(/\s+/g, " ")
            .trim();
    }

    function copyList(sourceList, document) {
        const targetList = document.createElement(sourceList.tagName.toLowerCase());

        Array.from(sourceList.children)
            .filter(element => element.tagName === "LI")
            .forEach(sourceItem => {
                const targetItem = document.createElement("li");
                targetItem.textContent = directText(sourceItem);

                Array.from(sourceItem.children)
                    .filter(element => listTags.has(element.tagName))
                    .forEach(childList => targetItem.append(copyList(childList, document)));

                targetList.append(targetItem);
            });

        return targetList;
    }

    function canonicalize(html) {
        const parsed = new DOMParser().parseFromString(html || "", "text/html");
        const firstList = parsed.body.querySelector("ol, ul");
        if (!firstList) {
            return "";
        }

        const output = document.createElement("div");
        output.append(copyList(firstList, document));
        return output.innerHTML;
    }

    function initializeEditor(container) {
        const source = container.querySelector(".toc-editor-source");
        const canvas = container.querySelector(".toc-editor-canvas");
        const form = container.closest("form");
        if (!source || !canvas) {
            return;
        }

        const initialHtml = canonicalize(source.value);
        canvas.innerHTML = initialHtml || "<ol><li><br></li></ol>";
        source.value = initialHtml;

        const synchronize = () => {
            source.value = canonicalize(canvas.innerHTML);
        };

        container.querySelectorAll("[data-command]").forEach(button => {
            button.addEventListener("mousedown", event => event.preventDefault());
            button.addEventListener("click", () => {
                canvas.focus();
                document.execCommand(button.dataset.command, false, null);
                synchronize();
            });
        });

        canvas.addEventListener("input", synchronize);
        canvas.addEventListener("paste", event => {
            event.preventDefault();
            document.execCommand("insertText", false, event.clipboardData?.getData("text/plain") || "");
            synchronize();
        });
        form?.addEventListener("submit", synchronize);
    }

    document.querySelectorAll("[data-toc-editor]").forEach(initializeEditor);
})();
