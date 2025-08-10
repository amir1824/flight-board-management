import type { FC } from "react";
import { Provider as ReduxStoreProvider } from "react-redux";
import { store } from "../store/store";
import type { Provider as ProviderProps } from "./provider"; 

export const ReduxProvider: FC<ProviderProps> = ({ children }) => {
  return <ReduxStoreProvider store={store}>{children}</ReduxStoreProvider>;
};
