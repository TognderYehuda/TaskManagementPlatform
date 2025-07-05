import React from 'react';
import { Link } from 'react-router-dom';

const Navbar = () => {
  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
      <div className="container">
        <Link className="navbar-brand" to="/">
          Task Management (React)
        </Link>
        <div className="navbar-nav">
          <Link className="nav-link" to="/">
            All Tasks
          </Link>
          <Link className="nav-link" to="/create">
            Create Task
          </Link>
        </div>
      </div>
    </nav>
  );
};

export default Navbar;