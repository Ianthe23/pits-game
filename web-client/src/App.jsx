import { BrowserRouter as Router, Routes, Route, Link } from "react-router-dom";
import "./App.css";
import GamePage from "./pages/GamePage";
import AdminPage from "./pages/AdminPage";

function App() {
  return (
    <Router>
      <div className="App">
        <nav className="App-header">
          <Link className="App-link" to="/">
            Play Game
          </Link>
          <Link className="App-link" to="/admin">
            Admin
          </Link>
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
