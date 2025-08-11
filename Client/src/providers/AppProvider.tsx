import type { FC } from "react";
import type { Provider } from "./provider";
import { QueryProvider } from "./QueryProvider";
import { ReduxProvider } from "./ReduxProvider";

const providers = [QueryProvider, ReduxProvider];

export const AppProvider: FC<Provider> = ({ children }) => {
  const app = providers.reduceRight(
    (nestedApp, CurrentProvider) => (
      <CurrentProvider>{nestedApp}</CurrentProvider>
    ),
    children
  );

  return <>{app}</>;
};