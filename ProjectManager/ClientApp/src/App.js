import React, {Component} from 'react';
import {Route} from 'react-router';
import {Layout} from './components/Layout';
import {Home} from './components/Home';
import {User} from './components/User';
import './custom.css'
import {Login} from "./components/Login";
import {request, Requests} from "./requests";
import "./components/CreateProject";
import {CreateProject} from "./components/CreateProject";
import {Project} from "./components/Project";
import {CreateIssue} from "./components/CreateIssue";
import {Issue} from "./components/Issue";
import {EditIssue} from "./components/EditIssue";

export default class App extends Component {
    static displayName = App.name;

    state = {
        logged: false
    }
    
    componentDidMount() {
        this.checkIfLogged()
        Requests.invalidSessionCallback = () => {
            this.setState({
                logged: false
            });
        }
    }

    render() {
        if (this.state.logged) {
            return (
                <Layout>
                    <Route exact path='/' component={Home}/>
                    <Route exact path='/create/issue' component={CreateProject}/>
                    <Route exact path='/view/project/:id' component={Project}/>
                    <Route exact path='/create/issue/:id' component={CreateIssue}/>
                    <Route exact path='/edit/issue/:id' component={EditIssue}/>
                    <Route exact path='/view/issue/:id' component={Issue}/>
                    <Route exact path='/view/user/:id' component={User}/>
                    <Route exact path='/view/user' component={User}/>
                </Layout>
            );
        } else {
            return (
                <Login
                    onLogin={this.onLogin}
                />
            )
        }
    }

    checkIfLogged = async () => {
        const response = await request('authentication','GET');
        const status = response.status;
        this.setState({
            logged: status === 200
        });
    }

    onLogin = () => {
        this.setState({
            logged: true
        })
    }
}
