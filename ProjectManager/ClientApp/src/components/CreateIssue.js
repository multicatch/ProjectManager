import React, {Component} from 'react';
import {Button, Form, FormFeedback, FormGroup, Input, Label, NavLink} from "reactstrap";
import {Link, Redirect} from "react-router-dom";
import {request} from "../requests";

export class CreateIssue extends Component {
    static displayName = CreateIssue.name;

    types = [
        'Requirement',
        'Task'
    ]

    statuses = [
        'Accepted',
        'InProgress',
        'Testing',
        'Resolved',
        'Rejected'
    ]

    state = {
        name: '',
        estimate: null,
        nameValid: true,
        nameFeedback: '',
        estimateValid: true,
        estimateFeedback: '',
        description: '',
        assignee: null,
        type: this.types[0],
        status: this.statuses[0],
        parent: null,
        submit: false
    }

    componentDidMount() {
        this.fetchProject(this.props.match.params.id)
    }

    componentWillReceiveProps = (nextProps) => {
        this.fetchProject(nextProps.match.params.id)
    }
    
    render() {
        const projectName = this.state.project ? this.state.project.name : '';
        const members = this.state.project
            ? this.memberOptions(this.state.project)
            : '';

        const types = this.types.map(type => <option key={"type-" + type} value={type}>{type}</option>);
        const statuses = this.statuses.map(status => <option key={"status-" + status} value={status}>{status}</option>);
        const issues = this.issuesOptions(this.state.issues);

        return (
            <div>
                <h1>New issue creation</h1>
                <p>Use this form to create a new issue for project {projectName}.</p>
                <Form onSubmit={this.create}>
                    <FormGroup>
                        <Label for="name">Name of issue</Label>
                        <Input type="text"
                               name="name"
                               id="name"
                               value={this.state.name}
                               onChange={this.setName}
                               invalid={!this.state.nameValid}
                        />
                        <FormFeedback>{this.state.nameFeedback}</FormFeedback>
                    </FormGroup>
                    <FormGroup>
                        <Label for="name">Description</Label>
                        <Input type="textarea"
                               name="description"
                               id="description"
                               value={this.state.description}
                               onChange={this.setDescription}
                        />
                    </FormGroup>
                    <FormGroup>
                        <Label for="name">Estimate Hours (optional)</Label>
                        <Input type="number"
                               name="description"
                               id="description"
                               value={this.state.estimate}
                               onChange={this.setEstimate}
                               invalid={!this.state.estimateValid}
                        />
                        <FormFeedback>{this.state.estimateFeedback}</FormFeedback>
                    </FormGroup>
                    <FormGroup>
                        <Label for="assigneeSelect">Assignee</Label>
                        <Input type="select" 
                               name="assignee" 
                               id="assigneeSelect" 
                               value={this.state.assignee} 
                               onChange={this.setAssignee}
                        >
                            {members}
                        </Input>
                    </FormGroup>
                    <FormGroup>
                        <Label for="typeSelect">Type</Label>
                        <Input type="select" 
                               name="type" id="typeSelect" 
                               value={this.state.type}
                               onChange={this.setType}
                        >
                            {types}
                        </Input>
                    </FormGroup>
                    <FormGroup>
                        <Label for="statusSelect">Status</Label>
                        <Input type="select" 
                               name="status" 
                               id="statusSelect" 
                               value={this.state.status}
                               onChange={this.setStatus}
                        >
                            {statuses}
                        </Input>
                    </FormGroup>
                    <FormGroup>
                        <Label for="statusSelect">Parent issue</Label>
                        <Input type="select"
                               name="parent"
                               id="parentSelect"
                               value={this.state.parent}
                               onChange={this.setParent}
                        >
                            {issues}
                        </Input>
                    </FormGroup>
                    <FormGroup inline>
                        <Button color="primary" onClick={this.create}>Create issue</Button>
                        <NavLink tag={Link} style={{display: 'inline-block'}}
                                 to={"/view/project/" + this.props.match.params.id}>
                            <Button color="primary" outline>Cancel</Button>
                        </NavLink>
                    </FormGroup>
                    {this.state.submit ? <Redirect to={"/view/project/" + this.props.match.params.id} /> : ''}
                </Form>
            </div>
        );
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
            project,
            assignee: user.name
        });
        
        return this.fetchIssues(project.id);
    }
    
    fetchIssues = async (id) => {
        const issuesResponse = await request('issues/project/' + id);
        if (issuesResponse.status !== 200) {
            return;
        }
        const body = await issuesResponse.json();
        this.setState({
            issues: body
        });
    }

    memberOptions = (project) => {
        return project.members.map(member =>
            <option key={"member-" + member} value={member}>{member}</option>
        );
    }

    issuesOptions = (issues) => {
        let list = [];
        if (issues) {
            list = issues.map(issue =>
                <option key={"issue-" + issue.id} value={issue.id}>#{issue.id} {issue.name}</option>
            );
        }
        list = [<option key={"issue-empty"} value={null}> - </option>, ...list];
        return list;
    }

    setName = (e) => {
        this.setState({
            name: e.target.value,
            nameValid: true
        });
    }

    setDescription = (e) => {
        this.setState({
            description: e.target.value
        });
    }

    setEstimate = (e) => {
        const number = e.target.value;
        this.setState({
            estimate: parseInt(number),
            estimateValid: true
        });
    }
    
    setAssignee = (e) => {
        this.setState({
            assignee: e.target.value
        })
    }

    setType = (e) => {
        this.setState({
            type: e.target.value
        })
    }
    
    setStatus = (e) => {
        this.setState({
            status: e.target.value
        })
    }
    
    setParent = (e) => {
        const value = e.target.value;
        if (value) {
            this.setState({
                parent: value
            });
        } else {
            this.setState({
                parent: null
            });
        }
    }

    create = async () => {
        let estimate = parseInt(this.state.estimate)

        const response = await request('issues/project/' + this.state.project.id, 'POST', {
            'Name': this.state.name,
            'Description': this.state.description,
            'EstimateHours': estimate,
            'Type': this.state.type,
            'Status': this.state.status,
            'Assignee': this.state.assignee,
            'Parent': parseInt(this.state.parent)
        });

        const body = await response.json();
        if (response.status === 200) {
            this.setState({
                submit: true
            })
            return;
        }
        
        if (!body.message) {
            return;
        }

        if (body.message.includes('Name')) {
            this.setState({
                nameValid: false,
                nameFeedback: body.message
            });
        }
        if (body.message.includes('Estimate')) {
            this.setState({
                estimateValid: false,
                estimateFeedback: body.message
            })
        }
    }
}
