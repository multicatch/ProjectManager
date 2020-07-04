import React, {Component} from 'react';
import {Route} from 'react-router';
import {Layout} from './components/Layout';
import {Home} from './components/Home';
import {FetchData} from './components/FetchData';
import {Counter} from './components/Counter';
import './custom.css'
import {Login} from "./components/Login";
import {request, Requests} from "./requests";

export default class App extends Component {
    static displayName = App.name;

    state = {
        logged: false
    }
    
    componentDidMount() {
        this.checkIfLogged()
        Requests.invalidSessionFallback = () => {
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
                    <Route path='/counter' component={Counter}/>
                    <Route path='/fetch-data' component={FetchData}/>
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
