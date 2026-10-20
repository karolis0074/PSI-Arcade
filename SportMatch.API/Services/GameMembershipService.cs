using SportMatch.API.Data;
using SportMatch.API.DataTransferObjects.Responses;
using SportMatch.API.Models;
using SportMatch.API.Repositories;
using SportMatch.API.Results;

namespace SportMatch.API.Services
{
    public class GameMembershipService
    {
        // game is almost full when 80% of spots are taken
        private const double AlmostFullRatio = 0.8;

        // DI — GameStore ateina per konstruktorių
        private readonly GameRepository _gameRepo;

        public GameMembershipService(GameRepository gameRepo)
        {
            _gameRepo = gameRepo;
        }

        public Game? GetGame(int gameId)
        {
            return _gameRepo.GetById(gameId);
        }

        public int GetFreeSpots(Game game)
        {
            return game.MaxPlayers - game.JoinedUsernames.Count;
        }

        public GameSpots? GetSpots(int gameId)
        {
            var game = _gameRepo.GetById(gameId);

            if (game is null)
                return null;

            var spots = new GameSpots();
            spots.Joined = game.JoinedUsernames.Count;
            spots.Max = game.MaxPlayers;
            spots.Free = GetFreeSpots(game);

            return spots;
        }

        public void UpdateStatus(Game game)
        {
            int joined = game.JoinedUsernames.Count;

            if (joined >= game.MaxPlayers)
                game.Status = GameStatus.Full;
            else if ((double)joined / game.MaxPlayers >= AlmostFullRatio)
                game.Status = GameStatus.AlmostFull;
            else
                game.Status = GameStatus.Open;
        }

        public MembershipResult Join(int gameId, string username)
        {
            var game = _gameRepo.GetById(gameId);

            if (game is null)
                return MembershipResult.GameNotFound;

            // can't join a game that already started
            if (game.StartTime <= DateTime.UtcNow)
                return MembershipResult.GameStarted;

            // user can't join the same game twice
            if (game.JoinedUsernames.Contains(username))
                return MembershipResult.AlreadyJoined;

            // no free spots left
            if (GetFreeSpots(game) <= 0)
                return MembershipResult.GameFull;

            game.JoinedUsernames.Add(username);
            UpdateStatus(game);

            _gameRepo.Update(game);

            return MembershipResult.Success;
        }

        public MembershipResult Leave(int gameId, string username)
        {
            var game = _gameRepo.GetById(gameId);

            if (game is null)
                return MembershipResult.GameNotFound;

             // can't leave a game that already started
            if (game.StartTime <= DateTime.UtcNow)
                return MembershipResult.GameStarted;

            // user has to be in the game to leave it
            if (!game.JoinedUsernames.Contains(username))
                return MembershipResult.NotJoined;

            game.JoinedUsernames.Remove(username);
            UpdateStatus(game);

            _gameRepo.Update(game);

            return MembershipResult.Success;
        }
    }
}