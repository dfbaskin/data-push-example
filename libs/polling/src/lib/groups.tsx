import { useQuery } from 'urql';
import { usePolling } from './usePolling';

const testQuery = `
{
  groups {
    name
    description
    drivers {
      name
      driverId
    }
  }
}
`;

interface Data {
  groups: {
    name: string;
    description: string;
    drivers: {
      name: string;
      driverId: string;
    }[]
  }[]
}

const defaultData: Data = {
  groups: []
};

export function Groups() {
  const [{data}, reexecuteQuery] = useQuery<Data>({
    query: testQuery,
    requestPolicy: 'network-only'
  });
  usePolling(() => {
    reexecuteQuery();
  });

  const groups = (data ?? defaultData).groups;
  const activeGroups = groups.filter(g => g.drivers.length !== 0);

  return (
    <div>
      <ul>

      {activeGroups.map(g => (
        <li key={g.name}>
          <span title={g.description}>
            {g.name}
            ({g.drivers.length})
          </span>
          <ul>
            {g.drivers.map(d => (
              <li>
                {d.driverId}
                ({d.name})
              </li>
            ))}
          </ul>
        </li>
      ))}
      </ul>
    </div>
  );
}

export default Groups;
