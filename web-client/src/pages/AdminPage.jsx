import React, { useState, useEffect } from "react";
import axios from "axios";
import "../styles/AdminPage.css";

const AdminPage = () => {
  const [pits, setPits] = useState([]);
  const [gameId, setGameId] = useState(null);
  const [newPit, setNewPit] = useState({ positionX: 0, positionY: 0 });

  useEffect(() => {
    loadPits();
  }, []);

  const loadPits = async () => {
    try {
      const response = await axios.get("http://localhost:5252/api/Admin/pits");
      setPits(response.data);
    } catch (error) {
      console.error("Error loading pits:", error);
    }
  };

  const addPit = async () => {
    if (!newPit.positionX || !newPit.positionY || !gameId) {
      alert("Please enter both position X and Y and select a game");
      return;
    }

    // Check if the game is completed
    const game = await axios.get(
      `http://localhost:5252/api/Admin/games/${gameId}`
    );
    if (!game.data.isCompleted) {
      alert("Game is not completed");
      return;
    }

    // Check if position already exists
    const positionExists = pits.some(
      (pit) =>
        pit.positionX === newPit.positionX &&
        pit.positionY === newPit.positionY &&
        pit.gameId === gameId
    );
    if (positionExists) {
      alert("Position already exists");
      return;
    }

    try {
      await axios.post(
        `http://localhost:5252/api/Admin/pits?gameId=${gameId}`,
        { positionX: newPit.positionX, positionY: newPit.positionY }
      );
      setNewPit({ positionX: 0, positionY: 0 });
      loadPits();
      alert("Pit added successfully");
    } catch (error) {
      console.error("Error adding pit:", error.response?.data || error.message);
      if (error.response?.status === 400) {
        alert(error.response.data || "Position already exists or invalid data");
      } else {
        alert("Failed to add pit");
      }
    }
  };

  return (
    <div className="admin-page">
      <h1>Game administration</h1>

      {/* Add new configuration form */}
      <div className="add-config-section">
        <h2>Add New Pit</h2>
        <div className="config-form">
          <input
            type="text"
            placeholder="Game ID"
            value={gameId}
            onChange={(e) => setGameId(e.target.value)}
          />
          <input
            type="number"
            placeholder="Position X (0-3)"
            min="0"
            max="3"
            value={newPit.positionX}
            onChange={(e) =>
              setNewPit({
                ...newPit,
                positionX: parseInt(e.target.value),
              })
            }
          />
          <input
            type="number"
            placeholder="Position Y (0-2)"
            min="0"
            max="2"
            value={newPit.positionY}
            onChange={(e) =>
              setNewPit({
                ...newPit,
                positionY: parseInt(e.target.value),
              })
            }
          />
          <button onClick={addPit}>Add Pit</button>
        </div>
      </div>

      {/* Existing Configurations */}
      <div className="configs-section">
        <h2>Pits</h2>
        <div className="configs-list">
          {pits.map((pit) => (
            <div key={pit.id} className="config-item">
              <span className="game-id">{pit.gameId}</span>
              <span className="position">
                ({pit.positionX}, {pit.positionY})
              </span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default AdminPage;
