import { uniqueFlight } from "../support/helpers";

describe("Realtime updates (SignalR)", () => {
  it("shows a newly created flight without reload", () => {
    cy.visit("/");
    cy.dataCy("flights-table").should("exist");

    const flightNumber = uniqueFlight("SR");

    // יוצר טיסה דרך ה-API; הקליינט אמור לקבל push דרך SignalR
    cy.request("POST", "/api/Flights", {
      flightNumber,
      destination: "AMS",
      gate: "D4",
      departureTime: new Date(Date.now() + 2 * 60 * 60 * 1000).toISOString(),
      status: "Scheduled",
    }).its("status").should("be.oneOf", [200, 201]);

    // ממתין להופעה בטבלה בלי ריענון
    cy.dataCy("flights-table").contains(flightNumber, { timeout: 15000 }).should("exist");
  });
});
