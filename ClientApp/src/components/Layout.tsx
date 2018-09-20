import * as React from 'react';
import { Container } from 'semantic-ui-react';

export class Layout extends React.Component {
  public render() {
    return (
      <Container>
        <div className="content">
          {this.props.children}
        </div>
      </Container>
    );
  }
}
