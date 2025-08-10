import axios, {
  type AxiosInstance,
  type AxiosRequestTransformer,
  type AxiosResponseTransformer,
} from "axios";
import dayjs, { Dayjs } from "dayjs";
import { useMemo } from "react";

const reqTx: AxiosRequestTransformer = (data) => {
  const s = (v: any): any =>
    dayjs.isDayjs(v) ? (v as Dayjs).toISOString()
    : Array.isArray(v) ? v.map(s)
    : v && typeof v === "object" ? Object.fromEntries(Object.entries(v).map(([k,val]) => [k, s(val)]))
    : v;
  return s(data);
};

const iso = /^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}(?:\.\d+)?(?:Z|[+\-]\d{2}:\d{2})$/;
const resTx: AxiosResponseTransformer = (data) => {
  const r = (v: any): any =>
    typeof v === "string" && iso.test(v) ? (dayjs(v).isValid() ? dayjs(v) : v)
    : Array.isArray(v) ? v.map(r)
    : v && typeof v === "object" ? Object.fromEntries(Object.entries(v).map(([k,val]) => [k, r(val)]))
    : v;
  return r(data);
};

export const createHttpClient = (baseURL = "/api"): AxiosInstance =>
  axios.create({
    baseURL,
    transformRequest: [reqTx, ...(axios.defaults.transformRequest as any)],
    transformResponse: [...(axios.defaults.transformResponse as any), resTx],
  });

export const useHttpClient = (baseURL = "/api"): AxiosInstance =>
  useMemo(() => createHttpClient(baseURL), [baseURL]);