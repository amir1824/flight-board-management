// cypress/support/commands.ts
/// <reference types="cypress" />

declare global {
  namespace Cypress {
    interface Chainable {
      dataCy(value: string): Chainable<JQuery<HTMLElement>>;
      apiCreateFlight(f: {
        flightNumber: string;
        destination: string;
        gate: string;
        departureTime: string; // ISO
      }): Chainable<Cypress.Response<any>>;
      apiDeleteFlight(id: number): Chainable<Cypress.Response<any>>;
      fillFlightForm(f: {
        flightNumber: string;
        destination: string;
        gate: string;
        departureTime: string; // datetime-local "YYYY-MM-DDThh:mm"
      }): Chainable<void>;
    }
  }
}

Cypress.Commands.add("dataCy", (value: string) => {
  // עם גרשיים כדי לטפל בערכים עם רווחים
  return cy.get(`[data-cy="${value}"]`);
});

Cypress.Commands.add("apiCreateFlight", (f) => {
  // API רץ על 8080 (docker-compose)
  return cy.request("POST", "http://localhost:8080/api/Flights", f);
});

Cypress.Commands.add("apiDeleteFlight", (id: number) => {
  return cy.request("DELETE", `http://localhost:8080/api/Flights/${id}`);
});

Cypress.Commands.add("fillFlightForm", (f) => {
  cy.dataCy("flight-number-input").clear().type(f.flightNumber);
  cy.dataCy("destination-input").clear().type(f.destination);
  cy.dataCy("gate-input").clear().type(f.gate);
  cy.dataCy("departureTime-input").clear().type(f.departureTime);
});

export {};
