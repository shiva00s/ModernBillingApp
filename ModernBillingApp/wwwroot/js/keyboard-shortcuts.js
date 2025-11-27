// Keyboard shortcuts for billing/POS operations
window.billingShortcuts = {
    init: function() {
        document.addEventListener('keydown', function(e) {
            // Only activate if not typing in an input field
            if (e.target.tagName === 'INPUT' || e.target.tagName === 'TEXTAREA' || e.target.isContentEditable) {
                return;
            }

            // Ctrl/Cmd + S: Save bill
            if ((e.ctrlKey || e.metaKey) && e.key === 's') {
                e.preventDefault();
                const saveButton = document.querySelector('button[type="submit"]');
                if (saveButton && !saveButton.disabled) {
                    saveButton.click();
                }
            }

            // F1: Focus on product search
            if (e.key === 'F1') {
                e.preventDefault();
                const productSelect = document.querySelector('select[class*="form-select"]');
                if (productSelect) {
                    productSelect.focus();
                }
            }

            // F2: Focus on customer search
            if (e.key === 'F2') {
                e.preventDefault();
                const customerInput = document.querySelector('input[placeholder*="name"]');
                if (customerInput) {
                    customerInput.focus();
                }
            }

            // F3: Clear bill
            if (e.key === 'F3') {
                e.preventDefault();
                const clearButton = document.querySelector('button:contains("Clear")');
                if (clearButton) {
                    clearButton.click();
                }
            }

            // Escape: Clear current form
            if (e.key === 'Escape') {
                const inputs = document.querySelectorAll('input[type="text"], input[type="number"]');
                inputs.forEach(input => {
                    if (input !== document.activeElement) {
                        input.blur();
                    }
                });
            }
        });
    }
};

