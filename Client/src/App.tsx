import { Dashboard } from "./Pages/FligthBoard";
import { AppProvider } from "./providers/AppProvider";

function App() {
  return (
    <AppProvider>
      <Dashboard />
    </AppProvider>
  );
}

export default App;
