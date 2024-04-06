/**
 * Import function triggers from their respective submodules:
 *
 * const {onCall} = require("firebase-functions/v2/https");
 * const {onDocumentWritten} = require("firebase-functions/v2/firestore");
 *
 * See a full list of supported triggers at https://firebase.google.com/docs/functions
 */

const {onRequest} = require("firebase-functions/v2/https");
const logger = require("firebase-functions/logger");

// Create and deploy your first functions
// https://firebase.google.com/docs/functions/get-started

exports.helloWorld = onRequest((request, response) => {
  logger.info("Hello logs!", {structuredData: true});
  response.send("Hello from Firebase!");
});


const functions = require("firebase-functions");
const admin = require("firebase-admin");

admin.initializeApp();

// Function to log user ID when added to the queue
exports.logUserAddedToQueue = functions.database.ref("/queues/{userId}")
    .onCreate((snapshot, context) => {
      const userId = context.params.userId;
      console.log(`User ${userId} added to the queue.`);
      return null;
    });


/**
 * Listens to changes in the MatchmakingQueue and initiates matchmaking process
 * when a player joins the queue.
 */
exports.matchmaking = functions.database.ref("queues/{playerId}")
    .onCreate((snapshot, context) => {
      const playerId = context.params.playerId;
      // const playerElo = snapshot.val().elo;

      // Find opponent based on elo
      return findOpponent(playerId)
          .then((opponentId) => {
            // Create match
            return createMatch(playerId, opponentId);
          })
          .catch((error) => {
            console.error("Error finding opponent:", error);
          });
    });

/**
 * Finds an opponent for the player based on their Elo rating.
 * @param {number} playerId - The Elo rating of the player.
 * @return {Promise<string|null>} - A Promise that resolves with the ID
 * of the opponent player, or null if no suitable opponent is found.
 */
function findOpponent(playerId) {
  // Logic to find opponent based on elo
  // For simplicity, let's just find any player in the matchmaking queue
  return admin.database().ref("queues").once("value")
      .then((snapshot) => {
        const players = snapshot.val();
        for (const Id in players) {
          if (Object.prototype.hasOwnProperty.call(players, Id)) {
            // const elo = players[playerId].elo;
            // Check if elo difference is within acceptable range
            // if (Math.abs(playerElo - elo) < 100) {
            if (Id != playerId) {
              return Id;
            // }
            }
          }
        }
        return null; // No suitable opponent found
      });
}

/**
 * Creates a match between two players and updates their status to "InLobby".
 * @param {string} playerId - The ID of the player initiating the match.
 * @param {string} opponentId - The ID of the opponent player.
 * @return {Promise<void>} - A Promise that resolves when the match is created.
 */
function createMatch(playerId, opponentId) {
  // Create match ID
  const matchId = admin.database().ref("matches").push().key;

  // Add players to match node
  const matchRef = admin.database().ref("matches").child(matchId);
  matchRef.child("players").child(playerId).set(true);
  matchRef.child("players").child(opponentId).set(true);
  // matchRef.child("status").set("waiting");

  // Set match ID for players
  admin.database().ref("players").child(playerId).child("matchID").set(matchId);
  admin.database().ref("players").child(opponentId)
      .child("matchID").set(matchId);

  // Remove players from matchmaking queue
  admin.database().ref("queues").child(playerId).remove();
  admin.database().ref("queues").child(opponentId).remove();

  // Update player status to "InLobby"
  admin.database().ref("players").child(playerId)
      .child("status").set("InLobby");
  admin.database().ref("players").child(opponentId)
      .child("status").set("InLobby");

  return Promise.resolve();
}

