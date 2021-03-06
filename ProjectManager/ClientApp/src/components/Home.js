import React, {Component} from 'react';
import {request} from "../requests";
import {Button, NavLink} from "reactstrap";
import {Link} from "react-router-dom";

export class Home extends Component {
    static displayName = Home.name;

    state = {
        user: {
            id: -1,
            name: ''
        },
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
                    <th>Creator</th>
                    <th>Hour Rate</th>
                    <th>No. Of Members</th>
                    {joinable ? <th>Join</th> : <th>Leave/Remove</th>}
                </tr>
                </thead>
                <tbody>
                {projects.map(project =>
                    <tr key={`project-${project.id}`}>
                        <td>{project.id}</td>
                        {this.projectName(project, !joinable)}
                        <td>{project.creator}</td>
                        <td>{project.hourValue}</td>
                        <td>{project.members.length}</td>
                        {joinable ?
                            <td><Button size="sm" onClick={() => this.joinProject(project.id)}>Join</Button></td>
                            : this.getLeaveButton(project)}
                    </tr>
                )}
                </tbody>
            </table>
        );
    }
    
    projectName = (project, enterable) => {
        if (enterable) {
            return <td><NavLink tag={Link} className="link-nopadding" to={`/view/project/${project.id}`}>{project.name}</NavLink></td>
        } else {
            return <td>{project.name}</td>
        }
    }

    getLeaveButton = (project) => {
        if (project.creator === this.state.user.name) {
            return <td><Button size="sm" color="danger" onClick={() => this.removeProject(project.id)}>Remove</Button></td>
        } else {
            return <td><Button size="sm" onClick={() => this.leaveProject(project.id)}>Leave</Button></td>
        }
    }

    fetchProjects = async () => {
        const userResponse = await request('user');
        if (userResponse.status !== 200) {
            return;
        }
        const user = await userResponse.json();
        this.setState({
            user
        });

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
                <p><NavLink tag={Link} className="text-dark" to="/create/issue"><Button color="primary">Create
                    project</Button></NavLink></p>
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
    
    removeProject = async (id) => {
        await request('projects/' + id, 'DELETE');
        return this.fetchProjects();
    }
}
