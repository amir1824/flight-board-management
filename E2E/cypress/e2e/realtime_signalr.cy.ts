import { uniqueFlight } from "../support/helpers";

describe("Realtime updates (SignalR)", () => {
  it("shows a newly created flight without reload", () => {
    cy.visit("/?e2e=1");                   
    cy.dataCy("flights-table").should("exist");

    cy.window()
      .its("__signalr.connected", { timeout: 10000 })
      .should("eq", true);

    const flightNumber = uniqueFlight("SR");

    cy.request("POST", "/api/Flights", {
      flightNumber,
      destination: "AMS",
      gate: "D4",
      departureTime: new Date(Date.now() + 2 * 60 * 60 * 1000).toISOString(),
      status: "Scheduled",
    })
      .its("status")
      .should("be.oneOf", [200, 201]);

    cy.window()
      .its("__signalr.lastAdded", { timeout: 15000 })
      .should((f: any) => expect(f?.flightNumber).to.eq(flightNumber));

    cy.dataCy("flights-table")
      .contains(flightNumber, { timeout: 10000 })
      .should("exist");
  });
});
