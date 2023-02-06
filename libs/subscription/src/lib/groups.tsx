import { GroupItem, GroupName } from '@example/dataui';
import { useSubscription } from 'urql';

const groupQuery = `
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
`;

const subscribeQuery = `
subscription {
  groupUpdated {
${groupQuery}
  }
}
`;

interface GroupData {
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
}

interface SubscriptionData {
  groupUpdated: GroupData;
}

interface Data {
  groups: GroupData[];
}

const defaultData: Data = {
  groups: [],
};

export function Groups() {
  const [{ data }] = useSubscription<SubscriptionData, Data>(
    {
      query: subscribeQuery,
    },
    (current = defaultData, data) => {
      return {
        ...current,
        groups: current.groups.reduce(
          (list, group) => {
            if (group.name !== data.groupUpdated.name) {
              list.push(group);
            }
            return list;
          },
          [data.groupUpdated]
        ),
      };
    }
  );

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
