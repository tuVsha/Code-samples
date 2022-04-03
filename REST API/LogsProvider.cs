using RestSharp;
using Saritasa.JiraDayIssues.Domain.Entities;
using Saritasa.JiraDayIssues.Infrastructure.Abstractions;
using Saritasa.JiraDayIssues.Infrastructure.Abstractions.Exceptions;
using NLog;

namespace Saritasa.JiraDayIssues.Infrastructure
{
    /// <summary>
    /// Provides getting logs from the https://api.tempo.io and https://saritasa.atlassian.net.
    /// </summary>
    public class LogsProvider
    {
        /// <summary>
        /// Provides logging.
        /// </summary>
        private Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Tempo website path.
        /// </summary>
        private readonly string tempoResource = "https://api.tempo.io/core/3/worklogs";

        /// <summary>
        /// Jira website path.
        /// </summary>
        private readonly string jiraPath = "https://saritasa.atlassian.net/rest/api/3/issue/";

        /// <summary>
        /// Initializes a new instance of the <see cref="LogsProvider"/> class.
        /// </summary>
        /// <param name="requestData">All needed data for request.</param>
        public LogsProvider(UserRequestData requestData)
        {
            this.RequestData = requestData;
        }

        /// <summary>
        /// All needed data for request.
        /// </summary>
        public UserRequestData RequestData { get; private set; }

        /// <summary>
        /// Provides getting logs from https://api.tempo.io for specified date.
        /// </summary>
        private async Task<TempoLogs> GetTempoLogsAsync(DateTime date, CancellationToken token)
        {
            var restClient = new RestClient();
            var restRequest = new RestRequest(tempoResource, Method.Get);
            restRequest.AddHeader("Authorization", $"Bearer {this.RequestData.TempoToken}");
            restRequest.AddParameter("from", date.ToString("yyyy-MM-dd"));
            restRequest.AddParameter("to", date.ToString("yyyy-MM-dd"));
            logger.Trace("Starting executing get to resource {tempoResource} for {date}.",
                tempoResource, date);
            var response = await restClient.ExecuteAsync(restRequest, token);
            var responseStatusCode = (int)response.StatusCode;
            logger.Trace("Response finished with code {responseStatusCode}", responseStatusCode);
            var responseContent = response.Content;
            if (responseStatusCode == 401)
            {
                throw new WrongResponseException("Not valid authentication data!", response);
            }
            if (responseStatusCode != 200)
            {
                throw new WrongResponseException("Some error occurs reaching the server.", response);
            }
            try
            {
                var tempoLogs = Newtonsoft.Json.JsonConvert.DeserializeObject<TempoLogs>(responseContent);
                logger.Trace("Successful get tempo logs in quantity: {count}.", tempoLogs.Results.Count);
                return tempoLogs;
            }
            catch (Newtonsoft.Json.JsonReaderException exception)
            {
                throw new WrongResponseException("Can not read server response.", exception);
            }
        }

        /// <summary>
        /// Provides getting logs from https://saritasa.atlassian.net.
        /// </summary>
        private async Task<JiraLogs> GetJiraLogsAsync(string taskId, CancellationToken cancellationToken)
        {
            var restClient = new RestClient();
            var jiraResource = jiraPath + taskId;
            var restRequest = new RestRequest(jiraResource, Method.Get);
            var authorization = System.Text.Encoding.UTF8.GetBytes($"{this.RequestData.Email}:{this.RequestData.JiraToken}");
            var authorization64 = System.Convert.ToBase64String(authorization);
            restRequest.AddHeader("Authorization", "Basic " + authorization64);
            logger.Trace("Starting executing get to resource {jiraResource}.",
                jiraResource);
            var a = restClient.ExecuteAsync(restRequest, cancellationToken);
            var response = await a;
            var responseContent = response.Content;
            var responseStatusCode = (int)response.StatusCode;
            logger.Trace("Response finished with code {responseStatusCode}", responseStatusCode);
            if (responseStatusCode == 401)
            {
                throw new WrongResponseException("Not valid authentication data!", response);
            }
            if (responseStatusCode != 200)
            {
                throw new WrongResponseException("Some error occurs reaching the server.", response);
            }
            try
            {
                var jiraLogs = Newtonsoft.Json.JsonConvert.DeserializeObject<JiraLogs>(responseContent);
                logger.Trace("Successful get jira logs for project with id: {project.Id}.", jiraLogs.Fields.Project.Id);
                return jiraLogs;
            }
            catch (Newtonsoft.Json.JsonReaderException exception)
            {
                throw new WrongResponseException("Can not read server response.", exception);
            }
        }

        /// <summary>
        /// Provides getting all tasks for specified date and specified user.
        /// </summary>
        /// <param name="date">Specified date.</param>
        public async Task<List<JiraProject>> GetJiraProjectsAsync(DateTime date, CancellationToken token)
        {
            var allTempoLogs = await this.GetTempoLogsAsync(date, token);
            var jiraTasks = new List<JiraTask>();
            var jiraProjects = new List<JiraProject>();
            foreach (var tempoLog in allTempoLogs.Results)
            {
                JiraTask jiraTask;
                var jiraTaskLogs = await this.GetJiraLogsAsync(tempoLog.Issue.Key.ToString(), token);
                jiraTask = new JiraTask(jiraTaskLogs.Id, jiraTaskLogs.Fields.Summary,
                                            jiraTaskLogs.Fields.TimeSpent,
                                            tempoLog.TimeSpentSeconds,
                                            this.RequestData,
                                            jiraTaskLogs.Fields.Project.Name,
                                            jiraTaskLogs.Fields.Project.Id);
                AddLogsToTasks(jiraTasks, tempoLog, jiraTask);
            }
            AddTasksToProjects(jiraProjects, jiraTasks);

            return jiraProjects;
        }

        /// <summary>
        /// Provides getting all worklogs for specified date.
        /// </summary>
        /// <param name="date">Specified date.</param>
        public async Task<TempoLogs> GetWorklogsAsync(DateTime date, CancellationToken token)
        {
            return await this.GetTempoLogsAsync(date, token);
        }

        /// <summary>
        /// Provides adding specified logs of task to current list of <see cref="JiraTask"/>.
        /// </summary>
        /// <param name="jiraTasks">The list to add to.</param>
        /// <param name="tempoLogs">Logs to check if its task already in list.</param>
        /// <param name="task">Task to add.</param>
        private void AddLogsToTasks(List<JiraTask> jiraTasks, TempoWorklogs tempoLogs, JiraTask task)
        {
            foreach (var alreadyInTask in jiraTasks)
            {
                if (alreadyInTask.IssueId == tempoLogs.Issue.Id.ToString())
                {
                    alreadyInTask.SpentTimePerDaySeconds += tempoLogs.TimeSpentSeconds;
                    alreadyInTask.SpentTimeAllSeconds += tempoLogs.TimeSpentSeconds;
                    return;
                }
            }
            jiraTasks.Add(task);
        }

        /// <summary>
        /// Provides adding list of tasks to list of projects.
        /// </summary>
        /// <param name="jiraProjects">List of projects.</param>
        /// <param name="jiraTasks">List of tasks.</param>
        private void AddTasksToProjects(List<JiraProject> jiraProjects, List<JiraTask> jiraTasks)
        {
            foreach (var jiraTask in jiraTasks)
            {
                AddTaskToProjects(jiraProjects, jiraTask);
            }
        }

        /// <summary>
        /// Provides adding specified task to list of projects.
        /// </summary>
        /// <param name="jiraProjects">List of projects.</param>
        /// <param name="jiraTask">Task to add.</param>
        private void AddTaskToProjects(List<JiraProject> jiraProjects, JiraTask jiraTask)
        {
            foreach (var jiraProject in jiraProjects)
            {
                if (jiraTask.ProjectId == jiraProject.Id)
                {
                    jiraProject.JiraTasks.Add(jiraTask);
                    return;
                }
            }
            jiraProjects.Add(new JiraProject(jiraTask.ProjectName, new List<JiraTask>() { jiraTask }, jiraTask.ProjectId));
        }
    }
}