import * as React from 'react';
import { Coffee } from './components/Coffee';
import { Layout } from './components/Layout';

export default class App extends React.Component {
  public render() {
    return (
      <Layout>
        <Coffee />
      </Layout>
    );
  }
}
