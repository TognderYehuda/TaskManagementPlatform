import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { tasksApi, usersApi, getStatusDescription } from '../services/api';

const TaskList = () => {
  const [tasks, setTasks] = useState([]);
  const [users, setUsers] = useState([]);
  const [selectedUserId, setSelectedUserId] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadUsers();
  }, []);

  useEffect(() => {
    if (selectedUserId) {
      loadUserTasks(selectedUserId);
    } else {
      loadAllTasks();
    }
  }, [selectedUserId]);

  const loadUsers = async () => {
    try {
      const response = await usersApi.getUsers();
      setUsers(response.data);
      if (response.data.length > 0) {
        setSelectedUserId(response.data[0].id);
      }
    } catch (err) {
      setError('Failed to load users');
    }
  };

  const loadAllTasks = async () => {
    try {
      setLoading(true);
      const response = await tasksApi.getAllTasks();
      setTasks(response.data);
    } catch (err) {
      setError('Failed to load tasks');
    } finally {
      setLoading(false);
    }
  };

  const loadUserTasks = async (userId) => {
    try {
      setLoading(true);
      const response = await tasksApi.getUserTasks(userId);
      setTasks(response.data);
    } catch (err) {
      setError('Failed to load user tasks');
    } finally {
      setLoading(false);
    }
  };

  const handleUserChange = (e) => {
    setSelectedUserId(e.target.value);
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString();
  };

  if (loading) return <div className="text-center">Loading...</div>;
  if (error) return <div className="alert alert-danger">{error}</div>;

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h1>Task Management</h1>
        <Link to="/create" className="btn btn-primary">
          Create New Task
        </Link>
      </div>

      <div className="mb-3">
        <label htmlFor="userSelect" className="form-label">
          Filter by User:
        </label>
        <select
          id="userSelect"
          className="form-select"
          value={selectedUserId}
          onChange={handleUserChange}
        >
          <option value="">All Tasks</option>
          {users.map(user => (
            <option key={user.id} value={user.id}>
              {user.name}
            </option>
          ))}
        </select>
      </div>

      {tasks.length === 0 ? (
        <div className="alert alert-info">
          <h4>No tasks found</h4>
          <p>There are no tasks to display.</p>
          <Link to="/create" className="btn btn-primary">
            Create your first task
          </Link>
        </div>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped">
            <thead>
              <tr>
                <th>Title</th>
                <th>Type</th>
                <th>Status</th>
                <th>Assigned To</th>
                <th>Created</th>
                <th>Closed</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {tasks.map(task => (
                <tr key={task.id} className={task.isClosed ? 'table-secondary' : ''}>
                  <td>{task.title}</td>
                  <td>{task.taskType}</td>
                  <td>
                    <span className={`badge ${task.isClosed ? 'bg-secondary' : 'bg-primary'}`}>
                      {getStatusDescription(task.taskType, task.currentStatus)}
                    </span>
                  </td>
                  <td>{task.assignedUser?.name || 'Unknown'}</td>
                  <td>{formatDate(task.createdAt)}</td>
                  <td>{task.closedAt ? formatDate(task.closedAt) : '-'}</td>
                  <td>
                    <Link
                      to={`/task/${task.id}`}
                      className="btn btn-sm btn-outline-primary"
                    >
                      View
                    </Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};

export default TaskList;