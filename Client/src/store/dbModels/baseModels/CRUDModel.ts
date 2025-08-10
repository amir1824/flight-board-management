import { Dayjs } from "dayjs";
import type { BaseModel } from "./BaseModel";

export interface CRUDModel extends BaseModel {
  createdAt: Dayjs;
  updatedAt: Dayjs;
}