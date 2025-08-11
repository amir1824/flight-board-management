import { lastFlightsUrl } from "../support/api";


const getParam = (u: URL, key: string) => u.searchParams.get(key) ?? "";

describe("Flights filters (URL-level)", () => {
  beforeEach(() => {
    cy.intercept("GET", /\/api\/Flights(\/search)?(\?.*)?$/).as("flights");

    cy.visit("/");
    cy.dataCy("flights-table").should("exist");
    cy.wait("@flights"); 
  });

  it("filters by destination only → ?status=&destination=russia", () => {
    cy.dataCy("filter-destination").clear().type("russia");
    cy.wait("@flights"); 

    lastFlightsUrl().then((u) => {
      expect(getParam(u, "destination")).to.eq("russia");
      expect(getParam(u, "status")).to.satisfy((v: string) => v === "" || v === null);
    });

    cy.dataCy("flights-table").find("tr").should("have.length.greaterThan", 1);
  });

  it("filters by status only → ?status=Scheduled&destination=", () => {
    cy.dataCy("filter-status").click();
    cy.get('ul[role="listbox"]').contains("Scheduled").click();
    cy.wait("@flights");

    lastFlightsUrl().then((u) => {
      expect(getParam(u, "status")).to.eq("Scheduled");
      expect(getParam(u, "destination")).to.satisfy((v: string) => v === "" || v === null);
    });
  });

  it("filters by both → ?status=Scheduled&destination=LAX", () => {
    cy.dataCy("filter-destination").clear().type("LAX");
    cy.wait("@flights");

    cy.dataCy("filter-status").click();
    cy.get('ul[role="listbox"]').contains("Scheduled").click();
    cy.wait("@flights");

    lastFlightsUrl().then((u) => {
      expect(getParam(u, "destination")).to.eq("LAX");
      expect(getParam(u, "status")).to.eq("Scheduled");
    });
  });

  it("clear filters → no destination/status params", () => {
    cy.dataCy("filter-destination").clear().type("LHR");
    cy.wait("@flights");
    cy.dataCy("filter-status").click();
    cy.get('ul[role="listbox"]').contains("Boarding").click();
    cy.wait("@flights");

    cy.dataCy("clear-filters").click();
    cy.wait("@flights"); 

    lastFlightsUrl().then((u) => {
      const url = u.toString();
      expect(url).not.to.include("destination=");
      expect(url).not.to.include("status=");
    });

    cy.dataCy("filter-destination").should("have.value", "");
    cy.dataCy("filter-status").find('input').should("have.value", "");
  });
});
