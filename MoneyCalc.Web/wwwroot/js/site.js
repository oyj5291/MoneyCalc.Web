document.addEventListener("DOMContentLoaded", () => {
  const digitsOnly = (value) => value.replace(/[^\d]/g, "");
  const formatMoney = (value) => {
    const integerPart = value.includes(".") ? value.split(".")[0] : value;
    const digits = digitsOnly(integerPart);
    return digits ? Number(digits).toLocaleString("ko-KR") : "";
  };

  document.querySelectorAll("form").forEach((form) => {
    const quickOptionButtons = form.querySelectorAll("[data-input][data-value]");
    const moneyInputs = form.querySelectorAll(".money-input");

    if (quickOptionButtons.length === 0 && moneyInputs.length === 0) {
      return;
    }

    const updateQuickOptionState = (input) => {
      form.querySelectorAll(`[data-input="${input.id}"]`).forEach((button) => {
        button.classList.toggle(
          "active",
          digitsOnly(input.value) === button.dataset.value
        );
      });
    };

    moneyInputs.forEach((input) => {
      input.value = formatMoney(input.value);
      updateQuickOptionState(input);

      input.addEventListener("focus", () => {
        input.value = digitsOnly(input.value);
      });

      input.addEventListener("input", () => {
        input.value = digitsOnly(input.value);
        updateQuickOptionState(input);
      });

      input.addEventListener("blur", () => {
        input.value = formatMoney(input.value);
      });
    });

    quickOptionButtons.forEach((button) => {
      button.addEventListener("click", () => {
        const input = form.querySelector(`#${button.dataset.input}`);

        if (!input) {
          return;
        }

        input.value = input.classList.contains("money-input")
          ? formatMoney(button.dataset.value)
          : button.dataset.value;
        input.dispatchEvent(new Event("change", { bubbles: true }));
        updateQuickOptionState(input);
      });
    });

    form.querySelectorAll("input:not(.money-input)").forEach((input) => {
      input.addEventListener("input", () => updateQuickOptionState(input));
      updateQuickOptionState(input);
    });

    form.addEventListener("submit", () => {
      moneyInputs.forEach((input) => {
        input.value = digitsOnly(input.value);
      });
    }, true);
  });

  const loanForm = document.querySelector("#loan-calculator-form");

  if (!loanForm) {
    return;
  }

  const repaymentTypeInput = loanForm.querySelector("#RepaymentType");
  const gracePeriodInput = loanForm.querySelector("#GracePeriodMonths");
  const gracePeriodField = loanForm.querySelector("#grace-period-field");

  const updateGracePeriodState = () => {
    const isBullet = repaymentTypeInput.value === "Bullet";
    gracePeriodField.classList.toggle("grace-period-disabled", isBullet);
    gracePeriodInput.disabled = isBullet;

    gracePeriodField.querySelectorAll("button").forEach((button) => {
      button.disabled = isBullet;
    });

    if (isBullet) {
      gracePeriodInput.value = "0";
    }
  };

  updateGracePeriodState();
  repaymentTypeInput.addEventListener("change", updateGracePeriodState);

  loanForm.addEventListener("submit", () => {
    gracePeriodInput.disabled = false;
  }, true);
});
