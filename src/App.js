import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';

import Navbar from './components/Navbar';
import TaskList from './components/TaskList';
import TaskDetails from './components/TaskDetails';
import CreateTask from './components/CreateTask';
import UserTasks from './components/UserTasks';

function App() {
  return (
    <Router>
      <div className="App">
        <Navbar />
        <div className="container mt-4">
          <Routes>
            <Route path="/" element={<TaskList />} />
            <Route path="/user/:userId" element={<UserTasks />} />
            <Route path="/task/:id" element={<TaskDetails />} />
            <Route path="/create" element={<CreateTask />} />
          </Routes>
        </div>
      </div>
    </Router>
  );
}

export default App;