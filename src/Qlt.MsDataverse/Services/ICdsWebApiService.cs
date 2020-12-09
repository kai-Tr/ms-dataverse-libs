using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qlt.MsDataverse.Services
{
    public interface ICdsWebApiService
    {
        /// <summary>
        /// Retrieves data from a specified resource.
        /// </summary>
        /// <param name="path">The path to the resource</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>The response from the request.</returns>
        JToken Get(string path, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Gets a typed response from a specified resource.
        /// </summary>
        /// <typeparam name="T">The type of response</typeparam>
        /// <param name="path">The path to the resource.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>The typed response from the request.</returns>
        T Get<T>(string path, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Retrieves data from a specified resource asychronously.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>The response to the request.</returns>
        Task<JToken> GetAsync(string path, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Gets a typed response from a specified resource asychronously
        /// </summary>
        /// <typeparam name="T">The type of resource</typeparam>
        /// <param name="path">The path to the resource.</param>
        /// <param name="headers"></param>
        /// <returns>Any custom headers to control optional behaviors.</returns>
        Task<T> GetAsync<T>(string path, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Posts a payload to the specified resource.
        /// </summary>
        /// <param name="path">The path to the resource</param>
        /// <param name="body">The payload to send.</param>
        /// <param name="headers">Any headers to control optional behaviors.</param>
        /// <returns>The response from the request.</returns>
        JObject Post(string path, object body, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Posts a payload to the specified resource asynchronously.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="body">The payload to send.</param>
        /// <param name="headers">Any headers to control optional behaviors.</param>
        /// <returns>The response from the request.</returns>
        Task<JObject> PostAsync(string path, object body, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Creates an entity and returns the URI
        /// </summary>
        /// <param name="entitySetName">The entity set name of the entity to create.</param>
        /// <param name="body">The JObject containing the data of the entity to create.</param>
        /// <returns>The Uri for the created entity record.</returns>
        Uri PostCreate(string entitySetName, object body);

        /// <summary>
        /// Creates an entity asynchronously and returns the URI
        /// </summary>
        /// <param name="entitySetName">The entity set name of the entity to create.</param>
        /// <param name="body">The JObject containing the data of the entity to create.</param>
        /// <returns>The Uri for the created entity record.</returns>
        Task<Uri> PostCreateAsync(string entitySetName, object body);

        /// <summary>
        /// Sends a PATCH request to update a resource.
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="body">The payload to send to update the resource.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        void Patch(Uri uri, object body, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Sends a PATCH request to update a resource asynchronously
        /// </summary>
        /// <param name="path">The path to the resource.</param>
        /// <param name="body">The payload to send to update the resource.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>Task</returns>
        Task PatchAsync(Uri uri, object body, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Deletes an entity
        /// </summary>
        /// <param name="path">The path to the resource to delete</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        void Delete(string path, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Deletes an entity asychronously
        /// </summary>
        /// <param name="path">The path to the resource to delete.</param>
        /// <param name="headers">Any custom headers to control optional behaviors.</param>
        /// <returns>Task</returns>
        Task DeleteAsync(string path, Dictionary<string, List<string>> headers = null);

        /// <summary>
        /// Updates a property of an entity
        /// </summary>
        /// <param name="path">The path to the entity.</param>
        /// <param name="property">The name of the property to update.</param>
        /// <param name="value">The value to set.</param>
        void Put(string path, string property, string value);

        /// <summary>
        /// Updates a property of an entity asychronously
        /// </summary>
        /// <param name="path">The path to the entity.</param>
        /// <param name="property">The name of the property to update.</param>
        /// <param name="value">The value to set.</param>
        /// <returns>Task</returns>
        Task PutAsync(string path, string property, string value);
    }
}
