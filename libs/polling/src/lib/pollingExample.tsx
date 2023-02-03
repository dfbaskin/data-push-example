import { DataUI } from '@example/dataui';

function ToDo() {
  return <div>To Do</div>;
}

export function PollingExample() {
  return (
    <DataUI
      groupsElement={<ToDo />}
      gridElement={<ToDo />}
      geolocationElement={<ToDo />}
      detailsElement={<ToDo />}
    />
  );
}

export default PollingExample;
