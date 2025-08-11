export const dtLocal = (minsFromNow = 180) => {
  const d = new Date(Date.now() + minsFromNow * 60_000);
  const pad = (n: number) => String(n).padStart(2, "0");
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
};

export const uniqueFlight = (prefix = "CY") =>
  `${prefix}${(Date.now() % 10_000).toString().padStart(4, "0")}`; 