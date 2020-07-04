import React, {Component} from 'react';
import {request} from "../requests";
import {Button, Form, FormGroup, NavLink} from "reactstrap";
import {Link, Redirect} from "react-router-dom";

export class Project extends Component {
    static displayName = Project.name;

    state = {
        user: {
            id: -1,
            name: ''
        },
        project: {
            creator: ''
        },
        goBack: false
    }

    componentDidMount() {
        this.fetchProject(this.props.match.params.id)
    }

    getLeaveButton = () => {
        if (this.state.project.creator === this.state.user.name) {
            return <td><Button size="sm" color="danger" onClick={() => this.removeProject(this.state.project.id)}>Remove</Button></td>
        } else {
            return <td><Button size="sm" onClick={() => this.leaveProject(this.state.project.id)}>Leave</Button></td>
        }
    }

    fetchProject = async (id) => {
        const userResponse = await request('user');
        if (userResponse.status !== 200) {
            return;
        }
        const user = await userResponse.json();
        this.setState({
            user
        });

        const projectsResponse = await request('projects/' + id);
        if (projectsResponse.status !== 200) {
            this.setState({
                goBack: true
            })
            return;
        }
        const project = await projectsResponse.json();

        this.setState({
            project
        });
    }

    render() {
        return (
            <div>
                {this.state.goBack ? <Redirect to="/"/> : ''}
                <h2>Project {this.state.project ? this.state.project.name : ''}</h2>
                <p>Id: {this.state.project ? this.state.project.id : ''}</p>
                <p>{this.getLeaveButton()}</p>
                <p>Members:</p>
                {this.memberList()}
            </div>
        );
    }
    
    memberList = () => {
        if (this.state.project && this.state.project.members) {
            const list = this.state.project.members.map(member =>
                <li key={"member-" + member}>{member}</li>
            )
            return <ul>{list}</ul>
        }
        return <em>No members.</em>
    }
    
    leaveProject = async (id) => {
        await request('projects/' + id + '/members', 'DELETE');
        this.setState({
            goBack: true
        })
    }

    removeProject = async (id) => {
        await request('projects/' + id, 'DELETE');
        this.setState({
            goBack: true
        })
    }
}
