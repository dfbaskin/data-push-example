import { GroupItem, GroupName } from '@example/dataui';
import { useQuery } from 'urql';
import { usePolling } from './usePolling';

const testQuery = `
{
  groups {
    name
    description
    count
    drivers {
      name
      driverId
      transport {
        vehicle {
          vehicleId
          vehicleType
        }
      }
    }
  }
}
`;

interface Data {
  groups: {
    name: string;
    description: string;
    count: number;
    drivers: {
      name: string;
      driverId: string;
      transport?: {
        vehicle: {
          vehicleId: string;
          vehicleType: 'Truck' | 'Van';
        };
      };
    }[];
  }[];
}

const defaultData: Data = {
  groups: [],
};

export function Groups() {
  const [{ data }, reexecuteQuery] = useQuery<Data>({
    query: testQuery,
    requestPolicy: 'network-only',
  });
  usePolling(() => {
    reexecuteQuery();
  });

  const groups = (data ?? defaultData).groups;
  const activeGroups = groups.filter((g) => g.count !== 0);

  return (
    <div>
      {activeGroups.map((g) => (
        <GroupName
          key={g.name}
          name={g.name}
          count={g.count}
          element={g.drivers.map((d) => (
            <GroupItem
              key={d.driverId}
              driverId={d.driverId}
              driverName={d.name}
              vehicleId={d.transport?.vehicle?.vehicleId ?? '?'}
              vehicleType={d.transport?.vehicle?.vehicleType}
            />
          ))}
        />
      ))}
    </div>
  );
}

export default Groups;
