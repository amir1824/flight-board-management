import { Dashboard } from "./Pages/Dashboard";
import { AppProvider } from "./providers/AppProvider";

function App() {
  return (
    <AppProvider>
      <Dashboard />
    </AppProvider>
  );
}

export default App;
