import * as signalR from "@microsoft/signalr";

class SignalRConnection {
  constructor() {
    this.connection = null;
  }

  async start() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5252/gameHub")
      .withAutomaticReconnect()
      .build();

    try {
      await this.connection.start();
      console.log("SignalR connection established");
      return true;
    } catch (error) {
      console.error("Error starting SignalR connection:", error);
      return false;
    }
  }

  async joinGameRoom(playerName) {
    if (this.connection) {
      await this.connection.invoke("JoinGameRoom", playerName);
    }
  }

  onRankingsUpdate(callback) {
    if (this.connection) {
      this.connection.on("RankingsUpdated", callback);
    }
  }

  onGameCompleted(callback) {
    if (this.connection) {
      this.connection.on("GameCompleted", callback);
    }
  }

  async stop() {
    if (this.connection) {
      await this.connection.stop();
    }
  }
}

export default new SignalRConnection();
