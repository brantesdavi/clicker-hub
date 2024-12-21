using Clicker.Enums;
using Clicker.Models;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace Game.Hubs
{
    public class GameHub : Hub
    {

        /*
         * Uitlizei o SignalR para criar um hub pois foi o que apresentou 
         * o melhor desempenho com multiplos clients, e também por sua simplicidade
         */
        private static readonly List<Player> players = new List<Player>();
        private static bool isGameStarted = false;
        private static bool isFirstTurn = true;
        private static Player currentPlayer; 

        /*
         * Gerencia entrada de jogadores
         */
        public async Task JoinGame(string playerName)
        {
            if (isGameStarted)
            {
                await NotifyGameAlreadyStarted();
                return;
            }

            var player = CreateNewPlayer(playerName);
            players.Add(player);

            await NotifyPlayerJoined(player);
            await UpdatePlayersList();

            await AssignHost(player);
            await HandleGameStartConditions();
        }

        /*
         * O lobby precisa ter no minimo duas pessoas pra começar
         */
        public async Task StartGame()
        {
            if (players.Count >= 2 && !isGameStarted)
            {
                StartNewGame();
                await NotifyGameStarted();
            }
        }

        /*
         * Faz a logica de pegar o player e trocar o turno
         */
        public async Task ClickButton(double time, string playerName)
        {
            var currentPlayer = GetPlayerByName(playerName);
            if (currentPlayer != null)
            {
                UpdatePlayerTime(currentPlayer, time);
                await HandleTurnChange();
            }
        }

        /*
         * Finaliza o game
         */
        public async Task EndGame()
        {
            if (!isGameStarted)
            {
                ResetGame();
                await NotifyGameOver();
            }
        }

        /*
         * Desliga algum jogador se ele for desconectado
         */
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await RemovePlayerOnDisconnect();
            await base.OnDisconnectedAsync(exception);
        }

        /*
         * Impede que novos usuarios entrem com uma partida em andamento
         */
        private async Task NotifyGameAlreadyStarted() =>
            await Clients.Caller.SendAsync("GameAlreadyStarted");

        
        private Player CreateNewPlayer(string playerName) =>
            new Player
            {
                ConnectionId = Context.ConnectionId,
                Name = playerName
            };

        private async Task NotifyPlayerJoined(Player player) =>
            await Clients.All.SendAsync("PlayerJoined", player);

        private async Task UpdatePlayersList() =>
            await Clients.All.SendAsync("UpdatePlayersList", players);

        /*
         * Define quem pode iniciar o jogo
         */
        private async Task AssignHost(Player player)
        {
            var isHost = players.Count == 1;
            await Clients.Caller.SendAsync("HostAssigned", isHost);
        }

        private async Task HandleGameStartConditions()
        {
            if (players.Count >= 2 && !isGameStarted)
            {
                await Clients.Caller.SendAsync("AllowStartGame");
            }
        }

        /*
         * Configura o basico para o jogo ser iniciado
         */
        private void StartNewGame()
        {
            isGameStarted = true;
            isFirstTurn = true;

            currentPlayer = players.FirstOrDefault(p => p.Status != PlayerStatus.Eliminated);

            if (currentPlayer != null)
            {
                Clients.All.SendAsync("TurnChanged", players, currentPlayer, isFirstTurn);
            }
        }

        /*
         * Inicia o jogo de todos os usuarios
         */
        private async Task NotifyGameStarted() =>
            await Clients.All.SendAsync("GameStarted");

        private Player GetPlayerByName(string playerName) =>
            players.FirstOrDefault(p => p.Name == playerName);

        /*
         * Regra de eliminação por tempo
         */
        private void UpdatePlayerTime(Player player, double time)
        {
            player.AccumulatedTime = time;

            if (player.AccumulatedTime > 30)
            {
                player.Status = PlayerStatus.Eliminated;
            }
        }

        /*
         * Gerencia o processo do jogo e condição de vitória
         */
        private async Task HandleTurnChange()
        {
            if (players.Count(p => p.Status != PlayerStatus.Eliminated) > 1)
            {
                var nextPlayer = GetNextPlayer();
                if (nextPlayer != null)
                {
                    isFirstTurn = false;
                    await Clients.All.SendAsync("TurnChanged", players, nextPlayer, isFirstTurn);
                }
            }
            else
            {
                var winner = players.FirstOrDefault(p => p.Status != PlayerStatus.Eliminated);
                await Clients.All.SendAsync("GameOver", winner);
                isGameStarted = false;
            }
        }

        /*
         * Escolhe o proximo jogador
         */
        private Player GetNextPlayer()
        {
            if (currentPlayer == null) return null;

            var currentPlayerIndex = players.IndexOf(currentPlayer);

            for (int i = 1; i < players.Count; i++)
            {
                int nextPlayerIndex = (currentPlayerIndex + i) % players.Count;

               
                if (players[nextPlayerIndex].Status != PlayerStatus.Eliminated)
                {
                    currentPlayer = players[nextPlayerIndex]; 
                    return currentPlayer;
                }
            }

            return null; 
        }

        /*
         * Prepara novo jogo
         */
        private async Task ResetGame()
        {
            isGameStarted = false;
            players.Clear();
        }

        /*
         * Sincroniza a interface dos jogadores com a finalização do jogo
         */
        private async Task NotifyGameOver() =>
            await Clients.All.SendAsync("GameOver", players);

        /*
         * Evita inconsistências no estado do jogo
         */
        private async Task RemovePlayerOnDisconnect()
        {
            var player = players.FirstOrDefault(p => p.ConnectionId == Context.ConnectionId);
            if (player != null)
            {
                players.Remove(player);
                await UpdatePlayersList();
            }
        }
    }
}
