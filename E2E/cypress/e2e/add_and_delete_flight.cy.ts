import * as dayjs from "dayjs";
import { uniqueFlight } from "../support/helpers";

describe("Add & Delete Flight", () => {
  beforeEach(() => {
    cy.visit("/");
    cy.dataCy("flights-table").should("exist");
  });

  it("creates a flight and then deletes it", () => {
    const flightNumber = uniqueFlight("DL");

    cy.intercept("POST", "/api/Flights").as("createFlight");
    cy.intercept("GET", "/api/Flights*").as("getFlights");
    cy.intercept("DELETE", /\/api\/Flights\/\d+$/).as("deleteFlight");

    // יצירה
    cy.dataCy("add-flight-button").click();

    cy.dataCy("flight-number-input").clear().type(flightNumber).blur();
    cy.dataCy("destination-input").clear().type("NYC").blur();
    cy.dataCy("gate-input").clear().type("B5").blur();

    // לוקחים את מה שהטופס שם כבר בשדה ה־datetime-local ומקדמים ל+3 שעות
    cy.dataCy("departureTime-input")
      .invoke("val")
      .then((cur) => {
        const dep = dayjs(String(cur)).add(180, "minute").format("YYYY-MM-DDTHH:mm");
        cy.dataCy("departureTime-input")
          .clear()
          .type(dep)
          .trigger("input")
          .trigger("change")
          .blur();
      });

    cy.dataCy("save-flight").should("not.be.disabled").click();

    cy.wait("@createFlight").its("response.statusCode").should("be.oneOf", [200, 201]);
    // לוודא שהדיאלוג נסגר
    cy.dataCy("add-flight-form").should("not.exist");
    // לבדוק הופעה בטבלה (Cypress יריץ ריטריי עד לטיימאאוט)
    cy.dataCy("flights-table").contains(flightNumber, { timeout: 10000 }).should("exist");

    // מחיקה
    cy.contains('[data-cy="flights-table"] tr', flightNumber, { timeout: 10000 })
      .within(() => cy.get('[data-cy^="delete-flight-"]').click());

    cy.dataCy("delete-dialog").should("be.visible");
    cy.dataCy("confirm-delete").click();

    cy.wait("@deleteFlight")
      .its("response.statusCode")
      .should("be.oneOf", [200, 204]);

    cy.dataCy("flights-table")
      .contains(flightNumber, { timeout: 10000 })
      .should("not.exist");
  });
});
