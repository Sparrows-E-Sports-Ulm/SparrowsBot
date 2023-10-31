using Sparrows.Bot.Models;

namespace Sparrows.Bot.Services {    
    /// <summary>
    /// The User Service exposes an API to get and register useres in the system
    /// </summary>
    public interface IUserService {
        /// <summary>
        /// Gets a user from their Discord ID
        /// </summary>
        /// <param name="id">Discord User ID</param>
        /// <returns>Matching User</returns>
        Task<User> Get(ulong id);

        /// <summary>
        /// Adds a new User to the service
        /// </summary>
        /// <param name="user">The user to add</param>
        Task Add(User user);

        /// <summary>
        /// Deletes a user from the service with the matiching discord id 
        /// </summary>
        /// <param name="id">The discord id of the user to be deleted</param>
        Task<bool> Delete(ulong id);

        /// <summary>
        /// Deletes a user from the service
        /// </summary>
        /// <param name="user">The user to delete</param>
        Task<bool> Delete(User user);

        /// <summary>
        /// Check if a certain user exists in the service
        /// </summary>
        /// <param name="user">User to check</param>
        /// <returns>True if user exists in the serivce</returns>
        Task<bool> Exists(User user);

        /// <summary>
        /// Check if a certain user exists in the service by their Discord ID
        /// </summary>
        /// <param name="id">Discord ID of the user</param>
        /// <returns>True if the user exists in the service</returns>
        Task<bool> Exists(ulong id);
    }
}

