import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { tasksApi, usersApi, getStatusDescription, getMaxStatus } from '../services/api';

const TaskDetails = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const [task, setTask] = useState(null);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  // Form state
  const [newStatus, setNewStatus] = useState('');
  const [assignedUserId, setAssignedUserId] = useState('');
  const [customFields, setCustomFields] = useState({});

  useEffect(() => {
    loadTask();
    loadUsers();
  }, [id]);

  const loadTask = async () => {
    try {
      const response = await tasksApi.getTask(id);
      setTask(response.data);
      setAssignedUserId(response.data.assignedUserId);
      setNewStatus(response.data.currentStatus);
      
      // Load custom fields
      const fields = {};
      response.data.customFields?.forEach(field => {
        fields[field.fieldName] = field.fieldValue;
      });
      setCustomFields(fields);
    } catch (err) {
      setError('Failed to load task');
    } finally {
      setLoading(false);
    }
  };

  const loadUsers = async () => {
    try {
      const response = await usersApi.getUsers();
      setUsers(response.data);
    } catch (err) {
      console.error('Failed to load users');
    }
  };

  const handleStatusChange = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    try {
      const statusData = {
        newStatus: parseInt(newStatus),
        assignedUserId: parseInt(assignedUserId),
        customFields,
      };

      await tasksApi.changeTaskStatus(id, statusData);
      setSuccess('Task status changed successfully!');
      setTimeout(() => {
        loadTask(); // Reload task data
      }, 1000);
    } catch (err) {
      setError(err.response?.data?.error || 'Failed to change task status');
    }
  };

  const handleCloseTask = async () => {
    if (!window.confirm('Are you sure you want to close this task?')) {
      return;
    }

    setError('');
    setSuccess('');

    try {
      await tasksApi.closeTask(id);
      setSuccess('Task closed successfully!');
      setTimeout(() => {
        loadTask(); // Reload task data
      }, 1000);
    } catch (err) {
      setError(err.response?.data?.error || 'Failed to close task');
    }
  };

  const handleCustomFieldChange = (fieldName, value) => {
    setCustomFields(prev => ({
      ...prev,
      [fieldName]: value,
    }));
  };

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString() + ' ' + new Date(dateString).toLocaleTimeString();
  };

  const renderCustomFieldsForm = () => {
    if (!task || task.isClosed) return null;

    const maxStatus = getMaxStatus(task.taskType);
    const fields = [];

    if (task.taskType === 'Procurement') {
      if (parseInt(newStatus) >= 2) {
        fields.push(
          <div key="procurement-2">
            <h6>Required for Status 2 (Supplier offers received):</h6>
            <div className="mb-3">
              <label className="form-label">Price Quote 1</label>
              <input
                type="text"
                className="form-control"
                value={customFields.PriceQuote1 || ''}
                onChange={(e) => handleCustomFieldChange('PriceQuote1', e.target.value)}
              />
            </div>
            <div className="mb-3">
              <label className="form-label">Price Quote 2</label>
              <input
                type="text"
                className="form-control"
                value={customFields.PriceQuote2 || ''}
                onChange={(e) => handleCustomFieldChange('PriceQuote2', e.target.value)}
              />
            </div>
          </div>
        );
      }
      
      if (parseInt(newStatus) >= 3) {
        fields.push(
          <div key="procurement-3">
            <h6>Required for Status 3 (Purchase completed):</h6>
            <div className="mb-3">
              <label className="form-label">Receipt</label>
              <input
                type="text"
                className="form-control"
                value={customFields.Receipt || ''}
                onChange={(e) => handleCustomFieldChange('Receipt', e.target.value)}
              />
            </div>
          </div>
        );
      }
    } else if (task.taskType === 'Development') {
      if (parseInt(newStatus) >= 2) {
        fields.push(
          <div key="development-2">
            <h6>Required for Status 2 (Specification completed):</h6>
            <div className="mb-3">
              <label className="form-label">Specification</label>
              <textarea
                className="form-control"
                rows="3"
                value={customFields.Specification || ''}
                onChange={(e) => handleCustomFieldChange('Specification', e.target.value)}
              />
            </div>
          </div>
        );
      }
      
      if (parseInt(newStatus) >= 3) {
        fields.push(
          <div key="development-3">
            <h6>Required for Status 3 (Development completed):</h6>
            <div className="mb-3">
              <label className="form-label">Branch Name</label>
              <input
                type="text"
                className="form-control"
                value={customFields.BranchName || ''}
                onChange={(e) => handleCustomFieldChange('BranchName', e.target.value)}
              />
            </div>
          </div>
        );
      }
      
      if (parseInt(newStatus) >= 4) {
        fields.push(
          <div key="development-4">
            <h6>Required for Status 4 (Distribution completed):</h6>
            <div className="mb-3">
              <label className="form-label">Version Number</label>
              <input
                type="text"
                className="form-control"
                value={customFields.VersionNumber || ''}
                onChange={(e) => handleCustomFieldChange('VersionNumber', e.target.value)}
              />
            </div>
          </div>
        );
      }
    }

    return fields;
  };

  if (loading) return <div className="text-center">Loading...</div>;
  if (error && !task) return <div className="alert alert-danger">{error}</div>;
  if (!task) return <div className="alert alert-warning">Task not found</div>;

  const maxStatus = getMaxStatus(task.taskType);

  return (
    <div className="row">
      <div className="col-md-8">
        <div className="card">
          <div className="card-header">
            <h5>{task.title}</h5>
          </div>
          <div className="card-body">
            {error && <div className="alert alert-danger">{error}</div>}
            {success && <div className="alert alert-success">{success}</div>}

            <dl className="row">
              <dt className="col-sm-3">Type:</dt>
              <dd className="col-sm-9">{task.taskType}</dd>
              
              <dt className="col-sm-3">Status:</dt>
              <dd className="col-sm-9">
                <span className={`badge ${task.isClosed ? 'bg-secondary' : 'bg-primary'}`}>
                  {getStatusDescription(task.taskType, task.currentStatus)}
                </span>
              </dd>
              
              <dt className="col-sm-3">Assigned To:</dt>
              <dd className="col-sm-9">{task.assignedUser?.name}</dd>
              
              <dt className="col-sm-3">Created:</dt>
              <dd className="col-sm-9">{formatDate(task.createdAt)}</dd>
              
              {task.isClosed && (
                <>
                  <dt className="col-sm-3">Closed:</dt>
                  <dd className="col-sm-9">{formatDate(task.closedAt)}</dd>
                </>
              )}
            </dl>

            {task.customFields && task.customFields.length > 0 && (
              <>
                <h6>Custom Fields:</h6>
                <dl className="row">
                  {task.customFields.map((field, index) => (
                    <React.Fragment key={index}>
                      <dt className="col-sm-3">{field.fieldName}:</dt>
                      <dd className="col-sm-9">{field.fieldValue}</dd>
                    </React.Fragment>
                  ))}
                </dl>
              </>
            )}
          </div>
        </div>

        {!task.isClosed && (
          <div className="card mt-3">
            <div className="card-header">
              <h5>Change Status</h5>
            </div>
            <div className="card-body">
              <form onSubmit={handleStatusChange}>
                <div className="row">
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="form-label">New Status</label>
                      <select
                        className="form-select"
                        value={newStatus}
                        onChange={(e) => setNewStatus(e.target.value)}
                      >
                        {Array.from({ length: maxStatus }, (_, i) => i + 1).map(status => (
                          <option key={status} value={status}>
                            {status} - {getStatusDescription(task.taskType, status)}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                  <div className="col-md-6">
                    <div className="mb-3">
                      <label className="form-label">Assign To</label>
                      <select
                        className="form-select"
                        value={assignedUserId}
                        onChange={(e) => setAssignedUserId(e.target.value)}
                      >
                        {users.map(user => (
                          <option key={user.id} value={user.id}>
                            {user.name}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>
                </div>

                {renderCustomFieldsForm()}

                <div className="mb-3">
                  <button type="submit" className="btn btn-primary">
                    Change Status
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}

        {!task.isClosed && task.currentStatus === maxStatus && (
          <div className="card mt-3">
            <div className="card-header">
              <h5>Close Task</h5>
            </div>
            <div className="card-body">
              <p>This task is ready to be closed.</p>
              <button
                className="btn btn-success"
                onClick={handleCloseTask}
              >
                Close Task
              </button>
            </div>
          </div>
        )}
      </div>

      <div className="col-md-4">
        <div className="card">
          <div className="card-header">
            <h5>Status History</h5>
          </div>
          <div className="card-body">
            {task.statusHistory && task.statusHistory.length > 0 ? (
              task.statusHistory
                .sort((a, b) => new Date(b.changedAt) - new Date(a.changedAt))
                .map((history, index) => (
                  <div key={index} className="mb-2">
                    <small className="text-muted">{formatDate(history.changedAt)}</small><br />
                    <strong>{history.assignedUser?.name}</strong><br />
                    Status: {history.fromStatus} → {history.toStatus}<br />
                    {history.notes && <em>{history.notes}</em>}
                    {index < task.statusHistory.length - 1 && <hr />}
                  </div>
                ))
            ) : (
              <p>No status changes recorded.</p>
            )}
          </div>
        </div>
      </div>

      <div className="mt-3">
        <button
          className="btn btn-secondary"
          onClick={() => navigate('/')}
        >
          Back to Tasks
        </button>
      </div>
    </div>
  );
};

export default TaskDetails;