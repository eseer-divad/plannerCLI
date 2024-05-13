using plannerCLI.Models;
using plannerCLI.Repositories;
using System.Collections.Generic;

namespace plannerCLI.Services
{
    public class TaskService
    {
        private readonly TaskRepository _taskRepository;

        public TaskService(TaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public void AddTask(StandardTaskModel task)
        {
            _taskRepository.AddTask(task);
        }

        public void UpdateTask(StandardTaskModel task)
        {
            _taskRepository.UpdateTask(task);
        }

        public void DeleteTask(int taskId)
        {
            _taskRepository.DeleteTask(taskId);
        }

        public StandardTaskModel GetTask(int id)
        {
            return _taskRepository.GetTask(id);
        }

        public List<StandardTaskModel> GetAllTasks()
        {
            return _taskRepository.GetAllTasks();
        }
    }
}
