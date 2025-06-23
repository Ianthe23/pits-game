import { useState, useEffect } from "react";
import signalRConnection from "../services/signalRConnection";
import gameApi from "../services/gameApi";
import GameBoard from "../components/GameBoard";
import "../styles/GamePage.css";

function GamePage() {
  const [playerName, setPlayerName] = useState("");
  const [currentGame, setCurrentGame] = useState(null);
  const [gameStarted, setGameStarted] = useState(false);
  const [gameCompleted, setGameCompleted] = useState(false);
  const [points, setPoints] = useState(0);
  const [gameResult, setGameResult] = useState(null);
  const [rankings, setRankings] = useState([]);
  const [message, setMessage] = useState(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    // Initialize SignalR connection
    const initSignalR = async () => {
      const connected = await signalRConnection.start();
      if (connected) {
        // Listen for rankings updates
        signalRConnection.onRankingsUpdate((updatedRankings) => {
          setRankings(updatedRankings);
        });

        // Listen for game completed events
        signalRConnection.onGameCompleted((gameResult) => {
          setMessage(
            `🎉 ${gameResult.playerName} finished the game with ${gameResult.points} points!`
          );
        });
      }
    };

    initSignalR();
    loadRankings();

    return () => {
      signalRConnection.stop();
    };
  }, []);

  const loadRankings = async () => {
    try {
      const rankingsData = await gameApi.getRankings();
      setRankings(rankingsData);
    } catch (error) {
      console.error("Error loading rankings:", error);
    }
  };

  const startGame = async () => {
    if (!playerName.trim()) {
      setMessage("Please enter a player name");
      return;
    }

    setLoading(true);
    try {
      const game = await gameApi.startGame(playerName);
      console.log("Game started:", game);
      setCurrentGame(game);
      setGameStarted(true);
      setGameCompleted(false);
      setPoints(0);
      setGameResult(null);
      setMessage(
        "Game started! Click on a cell on each row, from top to bottom, to make an attempt."
      );
      await signalRConnection.joinGameRoom(playerName);
    } catch (error) {
      console.error("Error starting game:", error);
      setMessage("Failed to start game. Please try again.");
    }
    setLoading(false);
  };

  const makeAttempt = async (positionX, positionY) => {
    if (!currentGame || !gameStarted || gameCompleted) {
      return;
    }

    console.log("Making attempt with:", {
      currentGame,
      positionX,
      positionY,
      gameId: currentGame?.gameId,
    });

    setLoading(true);
    try {
      const result = await gameApi.makeAttempt(
        currentGame.gameId,
        positionX,
        positionY
      );
      console.log("Attempt result:", result);
      setPoints(result.points);
      if (result.gameCompleted) {
        setMessage(
          `🎉 ${playerName} finished the game with ${result.points} points!`
        );
        setGameCompleted(true);
        setGameResult(result);
        await loadRankings();
      }
    } catch (error) {
      console.error("Error making attempt:", error);
      setMessage("Failed to make attempt. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  const resetGame = () => {
    setCurrentGame(null);
    setGameStarted(false);
    setGameCompleted(false);
    setPoints(0);
    setGameResult(null);
    setMessage("");
    setPlayerName("");
  };

  return (
    <div className="game-page">
      <header className="game-header">
        <h1>Gropi Game</h1>
        <p>Survive the road without hitting the pits!</p>
      </header>
      <main className="game-main">
        {!gameStarted ? (
          <div className="game-start">
            <h2>Enter your name to start the game</h2>
            <div className="game-start-form">
              <input
                type="text"
                placeholder="Your name..."
                value={playerName}
                onChange={(e) => setPlayerName(e.target.value)}
                onKeyPress={(e) => e.key === "Enter" && startGame()}
                disabled={loading}
              />
              <button onClick={startGame} disabled={loading}>
                {loading ? "Starting..." : "Start Game"}
              </button>
            </div>
          </div>
        ) : (
          <div className="game-info">
            <h2>Player: {playerName}</h2>
            <p>Points: {points}</p>
            {gameCompleted && (
              <button onClick={resetGame} className="game-new-game-btn">
                Start New Game
              </button>
            )}
          </div>
        )}

        <GameBoard
          onCellClick={makeAttempt}
          gameStarted={gameStarted}
          gameCompleted={gameCompleted}
        />

        {message && (
          <div
            className={`message ${
              gameResult?.isSuccessful ? "success" : "info"
            }`}
          >
            {message}
          </div>
        )}

        {gameResult && (
          <div className="game-result">
            <h3>Game Result</h3>
            <p>
              <strong>Points:</strong> {gameResult.points}
            </p>
            <p>
              <strong>Attempts:</strong>{" "}
              {gameResult.isCompleted ? gameResult.pitElements : "N/A"}
            </p>
          </div>
        )}
      </main>

      <aside className="ranking-section">
        <h3>Rankings</h3>
        {rankings.length === 0 ? (
          <p>No finished games yet!</p>
        ) : (
          <div className="ranking-list">
            {rankings.map((rank, index) => (
              <div key={rank.gameId} className="ranking-item">
                <span className="rank">#{index + 1}</span>
                <span className="player-name">{rank.playerName}</span>
                <span className="attempts">{rank.points}</span>
                <span className="durations-seconds">
                  {rank.durationSeconds}
                </span>
              </div>
            ))}
          </div>
        )}
      </aside>
    </div>
  );
}

export default GamePage;
