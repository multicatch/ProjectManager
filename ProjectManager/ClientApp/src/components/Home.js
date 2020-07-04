import React, {Component} from 'react';
import {request} from "../requests";
import {Button, NavLink} from "reactstrap";
import {Link} from "react-router-dom";

export class Home extends Component {
    static displayName = Home.name;
    
    state = {
        userProjects: null,
        otherProjects: null
    }

    componentDidMount() {
        this.fetchProjects()
    }

    renderProjectsTable = (projects, joinable = false) => {
        return (
            <table className='table table-striped' aria-labelledby="projectsTable">
                <thead>
                <tr>
                    <th>Id</th>
                    <th>Name</th>
                    <th>Hour Rate</th>
                    <th>No. Of Members</th>
                    {joinable ? <th>Join</th> : <th>Leave</th>}
                </tr>
                </thead>
                <tbody>
                {projects.map(project =>
                    <tr key={`project-${project.id}`}>
                        <td>{project.id}</td>
                        <td>{project.name}</td>
                        <td>{project.hourValue}</td>
                        <td>{project.members.length}</td>
                        {joinable ? <td><Button size="sm" onClick={() => this.joinProject(project.id)}>Join</Button></td> 
                            : <td><Button size="sm" onClick={() => this.leaveProject(project.id)}>Leave</Button></td>}
                    </tr>
                )}
                </tbody>
            </table>
        );
    }
    
    fetchProjects = async () => {
        const userResponse = await request('user');
        if (userResponse.status !== 200) {
            return;
        }
        const user = await userResponse.json();
        
        const projectsResponse = await request('projects');
        if (projectsResponse.status !== 200) {
            return;
        }
        const projects = await projectsResponse.json();
        
        const userProjects = projects.filter(project => project.members.includes(user.name))
        const otherProjects = projects.filter(project => !project.members.includes(user.name))
        
        this.setState({
            userProjects,
            otherProjects
        });
    }

    render() {
        const userProjects = this.state.userProjects
            ? this.renderProjectsTable(this.state.userProjects)
            : <p><em>Loading...</em></p>;


        const otherProjects = this.state.otherProjects
            ? this.renderProjectsTable(this.state.otherProjects, true)
            : <p><em>Loading...</em></p>;

        return (
            <div>
                <h1>Hello, world!</h1>
                <p>Welcome to ProjectManager. Here are your projects:</p>
                {userProjects}
                <p><NavLink tag={Link} className="text-dark" to="/createproject"><Button color="primary">Create project</Button></NavLink></p>
                <p>You can also join other projects:</p>
                {otherProjects}
            </div>
        );
    }
    
    joinProject = async (id) => {
        await request('projects/' + id, 'POST');
        return this.fetchProjects();
    }
    
    leaveProject = async (id) => {
        await request('projects/' + id + '/members', 'DELETE');
        return this.fetchProjects();
    }
}
