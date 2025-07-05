import axios from 'axios';

const API_BASE_URL = 'https://localhost:5001/api/';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Tasks API
export const tasksApi = {
  getAllTasks: () => api.get('/tasks'),
  getTask: (id) => api.get(`/tasks/${id}`),
  getUserTasks: (userId) => api.get(`/tasks/user/${userId}`),
  createTask: (taskData) => api.post('/tasks', taskData),
  changeTaskStatus: (id, statusData) => api.put(`/tasks/${id}/status`, statusData),
  closeTask: (id) => api.put(`/tasks/${id}/close`),
};

// Users API
export const usersApi = {
  getUsers: () => api.get('/users'),
};

// Helper function to get status description
export const getStatusDescription = (taskType, status) => {
  if (taskType === 'Procurement') {
    switch (status) {
      case 1: return 'Created';
      case 2: return 'Supplier offers received';
      case 3: return 'Purchase completed';
      default: return 'Unknown';
    }
  } else if (taskType === 'Development') {
    switch (status) {
      case 1: return 'Created';
      case 2: return 'Specification completed';
      case 3: return 'Development completed';
      case 4: return 'Distribution completed';
      default: return 'Unknown';
    }
  }
  return 'Unknown';
};

// Helper function to get max status
export const getMaxStatus = (taskType) => {
  return taskType === 'Procurement' ? 3 : taskType === 'Development' ? 4 : 1;
};