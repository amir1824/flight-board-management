// פונקציית עזר: זמן מקומי לפורמט YYYY-MM-DDTHH:mm
const dtLocal = (minsFromNow = 0) => {
  const d = new Date(Date.now() + minsFromNow * 60_000);
  const p = (n: number) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${p(d.getMonth()+1)}-${p(d.getDate())}T${p(d.getHours())}:${p(d.getMinutes())}`;
};

it("shows client-side validation errors", () => {
  cy.visit("/");
  cy.dataCy("add-flight-button").click();

  cy.dataCy("flight-number-input").type("x").clear().blur();
  cy.contains("Flight # is required").should("exist");

  cy.dataCy("destination-input").type("x").clear().blur();
  cy.contains("Destination is required").should("exist");

  // קובע תאריך עבר חוקי -> מפעיל את כלל ה"עתיד"
  cy.dataCy("departureTime-input").clear({ force: true }).type(dtLocal(-60)).blur();
  cy.contains("Must be in the future").should("exist");

  cy.dataCy("save-flight").should("be.disabled");
});
