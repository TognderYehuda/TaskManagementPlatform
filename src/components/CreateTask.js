import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { tasksApi, usersApi } from '../services/api';

const CreateTask = () => {
  const [title, setTitle] = useState('');
  const [taskType, setTaskType] = useState('');
  const [assignedUserId, setAssignedUserId] = useState('');
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      const response = await usersApi.getUsers();
      setUsers(response.data);
    } catch (err) {
      setError('Failed to load users');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!title || !taskType || !assignedUserId) {
      setError('Please fill in all fields');
      return;
    }

    setLoading(true);
    setError('');

    try {
      const taskData = {
        title,
        taskType,
        assignedUserId: parseInt(assignedUserId),
      };

      const response = await tasksApi.createTask(taskData);
      setSuccess('Task created successfully!');
      setTimeout(() => {
        navigate(`/task/${response.data.id}`);
      }, 1500);
    } catch (err) {
      setError(err.response?.data?.error || 'Failed to create task');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="row">
      <div className="col-md-6">
        <h1>Create New Task</h1>

        {error && <div className="alert alert-danger">{error}</div>}
        {success && <div className="alert alert-success">{success}</div>}

        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label htmlFor="title" className="form-label">
              Title *
            </label>
            <input
              type="text"
              id="title"
              className="form-control"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
            />
          </div>

          <div className="mb-3">
            <label htmlFor="taskType" className="form-label">
              Task Type *
            </label>
            <select
              id="taskType"
              className="form-select"
              value={taskType}
              onChange={(e) => setTaskType(e.target.value)}
              required
            >
              <option value="">Select task type...</option>
              <option value="Procurement">Procurement</option>
              <option value="Development">Development</option>
            </select>
          </div>

          <div className="mb-3">
            <label htmlFor="assignedUserId" className="form-label">
              Assign To *
            </label>
            <select
              id="assignedUserId"
              className="form-select"
              value={assignedUserId}
              onChange={(e) => setAssignedUserId(e.target.value)}
              required
            >
              <option value="">Select user...</option>
              {users.map(user => (
                <option key={user.id} value={user.id}>
                  {user.name}
                </option>
              ))}
            </select>
          </div>

          <div className="mb-3">
            <button
              type="submit"
              className="btn btn-primary"
              disabled={loading}
            >
              {loading ? 'Creating...' : 'Create Task'}
            </button>
            <button
              type="button"
              className="btn btn-secondary ms-2"
              onClick={() => navigate('/')}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>

      <div className="col-md-6">
        <div className="card">
          <div className="card-header">
            <h5>Task Type Information</h5>
          </div>
          <div className="card-body">
            <h6>Procurement Task Flow:</h6>
            <ol>
              <li>Created</li>
              <li>Supplier offers received (requires 2 price quotes)</li>
              <li>Purchase completed (requires receipt)</li>
              <li>Closed</li>
            </ol>

            <h6>Development Task Flow:</h6>
            <ol>
              <li>Created</li>
              <li>Specification completed (requires specification)</li>
              <li>Development completed (requires branch name)</li>
              <li>Distribution completed (requires version number)</li>
              <li>Closed</li>
            </ol>
          </div>
        </div>
      </div>
    </div>
  );
};

export default CreateTask;