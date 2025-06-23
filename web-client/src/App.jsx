import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import "./App.css";
import GamePage from "./pages/GamePage";
import AdminPage from "./pages/AdminPage";

function App() {
  return (
    <Router>
      <div>
        <nav>
          <Link to="/">Play Game</Link>
          <Link to="/admin">Admin</Link>
        </nav>

        <Routes>
          <Route path="/" element={<GamePage />} />
          <Route path="/admin" element={<AdminPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
