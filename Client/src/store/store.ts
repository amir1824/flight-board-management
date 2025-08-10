import { configureStore } from "@reduxjs/toolkit";
import flightsUi from "./flightsUiSlice";

export const store = configureStore({
  reducer: { flightsUi },
  devTools: true,
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
