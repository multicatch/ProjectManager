import React, {Component} from 'react';
import {request} from "../requests";
import {Project} from "./Project";

export class User extends Component {
    static displayName = User.name;

    state = {
        user: null,
        issues: null
    }

    componentDidMount() {
        if (this.props.match.params.id) {
            this.fetchUser(this.props.match.params.id);
        } else {
            this.fetchUser();
        }
    }
    
    componentWillReceiveProps = (nextProps) => {
        if (nextProps.match.params.id) {
            this.fetchUser(nextProps.props.match.params.id);
        } else {
            this.fetchUser();
        }
    }

    fetchUser = async (id = '') => {
        const userResponse = await request('user/' + id);
        if (userResponse.status !== 200) {
            return;
        }
        const user = await userResponse.json();
        this.setState({
            user
        });
        return this.fetchIssues(user);
    }

    fetchIssues = async (user) => {
        const issuesResponse = await request('issues/user/' + user.id);
        if (issuesResponse.status !== 200) {
            return;
        }
        const issues = await issuesResponse.json();
        this.setState({
            issues
        });
    }

    render() {
        const userName = this.state.user
            ? this.state.user.name
            : '';

        const projectList = this.state.user
            ? this.projectList(this.state.user.projects)
            : '';
        
        const issues = this.state.issues
            ? Project.issueTable(this.state.issues)
            : '';

        return (
            <div>
                <h1>User {userName}</h1>
                <p>In projects: </p>
                {projectList}
                <h4>User assigned issues</h4>
                {issues}
            </div>
        );
    }
    
    projectList = (projects) => {
        if (projects && projects.length > 0) {
            const list = projects.map(project =>
                <li key={"project-" + project.id}>{project.name} (id: {project.id})</li>
            )

            return <ul>{list}</ul>
        } else {
            return <p><em>Currently not in any project.</em></p>
        }
    }
}
