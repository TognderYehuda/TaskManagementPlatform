import React, { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { tasksApi, usersApi, getStatusDescription } from '../services/api';

const UserTasks = () => {
  const { userId } = useParams();
  const [tasks, setTasks] = useState([]);
  const [users, setUsers] = useState([]);
  const [currentUser, setCurrentUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadUsers();
  }, []);

  useEffect(() => {
    if (userId) {
      loadUserTasks(userId);
    }
  }, [userId]);

  const loadUsers = async () => {
    try {
      const response = await usersApi.getUsers();
      setUsers(response.data);
    } catch (err) {
      setError('Failed to load users');
    }
  };

  const loadUserTasks = async (userId) => {
    try {
      setLoading(true);
      const [tasksResponse, usersResponse] = await Promise.all([
        tasksApi.getUserTasks(userId),
        usersApi.getUsers()
      ]);
      
      setTasks(tasksResponse.data);
      const user = usersResponse.data.find(u => u.id === parseInt(userId));
      setCurrentUser(user);
    } catch (err) {
      setError('Failed to load user tasks');
    } finally {
      setLoading(false);
    }
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString();
  };

  if (loading) return <div className="text-center">Loading...</div>;
  if (error) return <div className="alert alert-danger">{error}</div>;

  return (
    <div>
      <h1>Tasks for {currentUser?.name || 'Unknown User'}</h1>

      <div className="mb-3">
        <label htmlFor="userSelect" className="form-label">Switch User:</label>
        <select
          id="userSelect"
          className="form-select"
          value={userId}
          onChange={(e) => window.location.href = `/user/${e.target.value}`}
        >
          {users.map(user => (
            <option key={user.id} value={user.id}>
              {user.name}
            </option>
          ))}
        </select>
      </div>

      <div className="mb-3">
        <Link to="/create" className="btn btn-primary">
          Create New Task
        </Link>
      </div>

      {tasks.length === 0 ? (
        <div className="alert alert-info">
          <h4>No tasks assigned</h4>
          <p>You don't have any tasks assigned to you yet.</p>
          <Link to="/create" className="btn btn-primary">
            Create a new task
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

export default UserTasks;