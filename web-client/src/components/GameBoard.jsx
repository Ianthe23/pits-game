import React from "react";
import "../styles/GameBoard.css";

const GameBoard = ({ onCellClick, gameStarted, gameCompleted }) => {
  const ROWS = 4;
  const COLS = 4;

  const renderCell = (row, col) => {
    const isClickable = gameStarted && !gameCompleted;

    return (
      <div
        key={`${row}-${col}`}
        className={`game-cell ${isClickable ? "clickable" : ""}`}
        onClick={() => isClickable && onCellClick(row, col)}
        title={`Position: (${row}, ${col})`}
      >
        {gameStarted && !gameCompleted && <div className="cell-content"></div>}
      </div>
    );
  };

  const renderBoard = () => {
    const board = [];

    for (let row = 0; row < ROWS; row++) {
      const rowCells = [];
      for (let col = 0; col < COLS; col++) {
        rowCells.push(renderCell(row, col));
      }
      board.push(
        <div key={row} className="game-row">
          {rowCells}
        </div>
      );
    }
    return board;
  };

  return (
    <div className="game-board">
      <h3>Gropi Game Board</h3>
      <div className="board-container">{renderBoard()}</div>
      {!gameStarted && (
        <p className="instruction">
          Enter your name and click Start Game to begin!
        </p>
      )}
    </div>
  );
};

export default GameBoard;
