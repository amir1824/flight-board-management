import { Interception } from "cypress/types/net-stubbing";

export const createFlight = (overrides = {}) => {
  const body = {
    flightNumber: `DL${Math.floor(1000 + Math.random()*9000)}`,
    destination: "LAX",
    gate: "B5",
    departureTime: new Date(Date.now()+3*60*60*1000).toISOString(),
    status: "Scheduled",
    ...overrides,
  };
  return cy.request("POST", "/api/Flights", body).then(r => r.body);
};

export const deleteFlightById = (id: number) =>
  cy.request("DELETE", `/api/Flights/${id}`);


export const lastFlightsUrl = () =>
  cy.get<Interception[]>("@flights.all").then((calls) => {
    expect(calls.length, "at least one flights call").to.be.greaterThan(0);
    const last = calls[calls.length - 1]; // בלי .at כדי לא להסתבך עם ES5
    return new URL(last.request.url, window.location.origin);
  });