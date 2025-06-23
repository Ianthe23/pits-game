import axios from "axios";

const API_BASE_URL = "http://localhost:5252/api";

const gameApi = {
  startGame: async (playerName) => {
    const response = await axios.post(`${API_BASE_URL}/Game/start`, {
      playerName: playerName,
    });
    return response.data;
  },
  makeAttempt: async (gameId, positionX, positionY) => {
    const response = await axios.post(`${API_BASE_URL}/Game/attempt`, {
      gameId: gameId,
      positionX: positionX,
      positionY: positionY,
    });
    return response.data;
  },
  getRankings: async () => {
    const response = await axios.get(`${API_BASE_URL}/Game/rankings`);
    return response.data;
  },
};

export default gameApi;
