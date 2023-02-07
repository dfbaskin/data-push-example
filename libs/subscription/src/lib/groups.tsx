import { GroupItem, GroupName } from '@example/dataui';
import { useQuery, useSubscription } from 'urql';

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

const groupsQuery = `
{
  groups {
${groupQuery}
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
  const [{ data: queryData, fetching }] = useQuery<Data>({
    query: groupsQuery,
  });

  const [{ data: subscriptionData }] = useSubscription<SubscriptionData, Data>(
    {
      query: subscribeQuery,
      pause: fetching,
    },
    (
      current = {
        groups: (queryData ?? defaultData).groups,
      },
      data
    ) => {
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

  const { groups } = subscriptionData ?? queryData ?? defaultData;
  const activeGroups = groups
    .filter((g) => g.count !== 0)
    .sort((a, b) => a.name.localeCompare(b.name));

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
