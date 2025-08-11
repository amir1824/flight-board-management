
export const uniqueFlight = (prefix = "CY") =>
  `${prefix}${(Date.now() % 10_000).toString().padStart(4, "0")}`; 