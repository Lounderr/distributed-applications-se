console.log("Custom Swagger JS loaded.");
(function () {
    const originalFetch = window.fetch;
    window.fetch = async function () {
        const response = await originalFetch.apply(this, arguments);

        if (arguments[0].includes("/login") && response.ok) {
            const clone = response.clone();
            const data = await clone.json();
            const token = data.accessToken;

            if (token) {
                let openAuthFormButton = document.querySelector(".auth-wrapper .authorize");
                openAuthFormButton.click();

                if (openAuthFormButton.classList.contains('locked')) {
                    setTimeout(function () {
                        let logoutButton = document.querySelector(".auth-btn-wrapper button.btn.modal-btn.auth.button");
                        logoutButton.click();
                    }, 500);
                }

                setTimeout(function () {
                    let tokenInput = document.querySelector(".auth-container input");
                    let authButton = document.querySelector(".auth-btn-wrapper .modal-btn.auth");
                    let closeButton = document.querySelector("button.btn-done");


                    let nativeInputValueSetter = Object.getOwnPropertyDescriptor(window.HTMLInputElement.prototype, "value").set;
                    nativeInputValueSetter.call(tokenInput, token);

                    let inputEvent = new Event('input', { bubbles: true });
                    tokenInput.dispatchEvent(inputEvent);
                    authButton.click();
                    closeButton.click();
                }, 500);
            }
        }

        return response;
    };
})();
