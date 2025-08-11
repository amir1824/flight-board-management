import { uniqueFlight } from "../support/helpers";

describe("Realtime updates (SignalR)", () => {
  it("shows a newly created flight without reload", () => {
    cy.visit("/?e2e=1");                    // מפעיל מצב e2e קטן באפליקציה
    cy.dataCy("flights-table").should("exist");

    // חכה שהחיבור באמת מוכן
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

    // וידוא שקיבלנו את האירוע (לא רק DOM)
    cy.window()
      .its("__signalr.lastAdded", { timeout: 15000 })
      .should((f: any) => expect(f?.flightNumber).to.eq(flightNumber));

    // עכשיו גם ב־DOM
    cy.dataCy("flights-table")
      .contains(flightNumber, { timeout: 10000 })
      .should("exist");
  });
});
